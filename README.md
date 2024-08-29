# DdadduClient

네이버 스마트스토어 ‘김따뚜의 일본어원서’ 상품 등록 자동화 프로그램입니다. 스토어와 계약된 일본 도서 회사 e-hon의 홈페이지에서 정보를 가져옵니다.

[![메인.png](https://i.postimg.cc/FR2nvqkW/image.png)](https://postimg.cc/mzySNmwC)

## 개요
e-hon의 랭킹페이지의 출판물을 가져와 업로드 양식에 맞는 엑셀을 만들어 줍니다. 도서 잡지 등 200개의 상품을 10분만에 올릴 수 있습니다.

[![랭킹.png](https://i.postimg.cc/3xP9gtsT/fodzld.png)](https://postimg.cc/w1QhzQxf)[![결과물.png](https://i.postimg.cc/GhGKZT5P/image.png)](https://postimg.cc/nCVqmrZM)

## 기능

### 1. 단순 스크래핑
출판물의 기본 정보는 물론 상세페이지까지 누락없이 전부 가져옵니다. 

[![원본.png](https://i.postimg.cc/VvbgsJL8/image.png)](https://postimg.cc/V0mnDLCG)
[![결과물.png](https://i.postimg.cc/pXKNqTQ4/image.png)](https://postimg.cc/GH3Spb5x)

왼쪽이 원본 홈페이지, 오른쪽이 스크래핑해서 재구성한 상세페이지입니다. 잘 가져왔죠? 
스크래핑이 어려운 기술은 아니지만 20개를 빠르게 처리 위해서 비동기 병렬 처리를 구현했고 코드를 깔끔하게 적기위해 전략 패턴을 적용했습니다. <br>
자세한 코드는 [ScraperDll 깃허브](https://github.com/ssongone/ScraperDll)

### 2. 한번에 여러 페이지 작업 - 페이지와 로직 분리하기

[![2024-08-28-211324.gif](https://i.postimg.cc/sfcHXhcV/2024-08-28-211324.gif)](https://postimg.cc/231793MJ)

클라이언트 프로그램을 만들면서 가장 신경을 쓴 부분입니다. 하나를 처리하는동안 프로그램 화면이 멈추면 너무 지루할 것 같아서 페이지와 로직을 분리하였습니다. 페이지를 이동할 때 무조건 새로운 뷰모델을 만드는 것이 아니라 딕셔너리를 통해 뷰모델을 가져오는 식으로 만들었습니다. 싱글톤을 살짝 변형한 느낌으로다가 구현해보았습니다. 

``` C#
    private static readonly Dictionary<(bool, int), DetailViewModel> _instances = new();
    public static DetailViewModel GetInstance(bool isBook, int option)
    {
        if (!_instances.ContainsKey((isBook, option)))
        {
            _instances[(isBook, option)] = new DetailViewModel(isBook, option);
        }
        return _instances[(isBook, option)];
    }
```

### 3. 이미지 호스팅

e-hon의 메인이미지는 avif 형식이었지만 네이버 스토어에서 avif를 지원하지 않더라구요. 그대로 긁어와서 올리면 오류가 발생하기 때문에 예전엔 하나하나 다운받아서 수동으로 네이버에 업로드해서 사용했어서 시간이 많이 걸렸었죠. 

이걸 해결하고 싶어서 직접 이미지 호스팅 서버를 만들었습니다. 서버로 쓸 컴퓨터를 6만원에 당근해오고 도메인도 500원에 구해왔습니다. 

[![do.png](https://i.postimg.cc/fTv3NC7v/do.png)](https://postimg.cc/ykk6hFbJ)
[![default.png](https://i.postimg.cc/9QFDF9JH/default.png)](https://postimg.cc/6ygWMykM)


자세한 서버 이야기는 요기서 만나보시죠!
[DdadduServer 깃허브](https://github.com/ssongone/DdadduServer)


### 4. 중복 확인 & 필터링
아래 코드는 버튼을 눌렀을 때 시작되는 메인 서비스 로직입니다. 총 5가지 단계로 진행됩니다.
    
    1️⃣ 리스트 로드 
    2️⃣ 중복확인 및 필터링
    3️⃣ 상품 디테일 페이지 스크래핑
    4️⃣ 이미지 확장자 변경
    5️⃣ 엑셀 파일 변환

1차적으로 랭킹페이지에서 제목이랑 상세페이지 url만 가져오는 데 이때 가져온 제목으로 중복과 필터링을 수행합니다.

``` C#
private async void Run()
{
    if (IsRunning)
    {
        await Application.Current.MainPage.DisplayAlert("경고", "현재 작업이 진행 중입니다.", "확인");
        return;
    }

    IsRunning = true;
    // 1. 리스트 로드
    StatusMessage = "목록을 가져오는 중";
    await LoadItems();

    // 2. 중복 확인 (서버 연동)
    StatusMessage = "목록을 확인하는 중";
    var (success, summaries) = await _apiRequestService.ValidatePublications(Items);
    
    // 3. 상품디테일 가져오기
    StatusMessage = "상세 정보를 가져오는 중";
    var publications = await _scraperService.GetPublications(summaries);

    // 4. 사진 확장자 변경 (서버 연동)
    StatusMessage = "사진 확장자를 변경하는 중";
    success = await _apiRequestService.UpdateMainImageUrls(publications);
    if (!success)
    {
        StatusMessage = "오류 발생 🙊 ";
        IsRunning = false;
        return;
    }

    // 5. 엑셀파일화
    StatusMessage = "엑셀 파일로 바꾸는 중";
    await Task.Run(() => _scraperService.ExportPublications(publications));

    StatusMessage = "✅✅✅";
    IsRunning = false;
}
```
랭킹 페이지엔 보통 스테디셀러가 많아 중복도서가 올라가는 경우가 많았습니다. 중복된 상품을 올리면 네이버에서 경고를 때리기 때문에 서버를 만든김에 겸사겸사 디비도 만들어서 중복을 검사할 수 있도록 했습니다. 

또 아무래도 일본 잡지를 가져오다보니 성인 잡지가 많은데 이걸 스토어에 올리면 역시 정지를 먹겠죠? 주인장에게 필터링 목록을 받아 거르는 작업을 추가합니다.

필터링 결과는 이런식으로 UI에 표시되고 후에 진행되는 작업에서 배제됩니다.

[![2024-08-26-234343.png](https://i.postimg.cc/Qx6Xmykv/2024-08-26-234343.png)](https://postimg.cc/3dv5J9qZ)

### 5. 자잘한 설정

주인장의 요구사항이 자주 바뀌는데 이럴때마다 클라이언트와 DLL 코드를 고치는 건 너무 귀찮죠. 자주 바뀔법한 것들을 설정페이지로 제공했습니다.

[![tjfwjd.png](https://i.postimg.cc/y8kSy99X/tjfwjd.png)](https://postimg.cc/K1hzZ10R)


## 별첨. MAUI 이야기
MAUI로 어플리케이션을 처음 만들어보았습니다. 윈폼이 저에겐 좀 구식으로 느껴졌고 MAUI가 크로스플랫폼인 점도 마음에 들어 MAUI를 선택했습니다. 윈도우에서 한걸음 더 나아가고 싶어하는 MS의 새로운 시도 같아서 호기심이 생기기도 했구요. 어떻게 크로스플랫폼 네이티브 앱을 구현했는지 궁금하기도 했습니다. 제 맥북 노트북에서 한번 실행시켜보고싶었지만 지금 윈도우 배포도 식은땀이 나서 크로스플랫폼 체험은 후순위로 미루겠습니다.

아무튼 C#과 XML로 뷰와 내부로직을 전부 만들 수 있어서 매우 편했던것 같아요. 제가 프론트엔드 전문가가 아니다보니 뭐가 부족한지는 정확히 모르겠습니다만 저같은 사람이 이정도의 작업물을 만드는데는 문제없는 수준인 것 같습니다. 자바로 짰으면 또 프론트 쪽 코드 짜느라 힘들었을 것 같아요. 윈폼에서 구현하기 힘든 페이지스택? 같은것도 쉽게 쓸 수 있어서 좋았어요. 앞으로 또 어떤 외주 작업을 하게 된다면 MAUI를 선택할 것 같아요. 정말 좋은 경험이었습니다. 
