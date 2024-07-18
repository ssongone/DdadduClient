using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DdadduBot.ViewModels
{
    public class Item
    {
        public string Title { get; set; }
        public string Status { get; set; }
    }

    public class MyViewModel
    {
        private List<Item> _items;

        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
            }
        }

        // 바인딩 해놓으면 페이지 로드 될때 자동 호출
        public MyViewModel()
        {
            LoadItems();
        }

        private void LoadItems()
        {
            Items = new List<Item>
        {
            new Item { Title = "ㅅㅇ", Status = "processing" },
            new Item { Title = "ㅁㅅ", Status = "processing" },
            new Item { Title = "ㅂㄱㅅ", Status = "start" },
            new Item { Title = "ㅇㅇ", Status = "start" },
            new Item { Title = "ㄷㄷ", Status = "completed" },
        };
        }
    }

}
