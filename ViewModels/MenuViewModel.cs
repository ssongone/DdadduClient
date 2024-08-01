using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DdadduBot.ViewModels
{
    public class Item
    {
        public string Title { get; set; }
        public string Status { get; set; }
    }

    public class MenuViewModel
    {
        private List<Item> _items;
        public bool IsBook;

        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
            }
        }
        public MenuViewModel()
        {
            LoadItems();
        }

        // 바인딩 해놓으면 페이지 로드 될때 자동 호출
        public MenuViewModel(bool isBook)
        {
            IsBook = isBook;
            if (isBook)
            {
                LoadItems();
            } else
            {
                LoadItems2();
            }
            
        }

        private void LoadItems()
        {
            Items = new List<Item>
        {
            new Item { Title = "소설·에세이", Status = "Go" },
            new Item { Title = "논픽션·교양", Status = "Go" },
            new Item { Title = "문고·신서", Status = "Go" },
            new Item { Title = "사상·심리·역사·교육", Status = "Go" },
            new Item { Title = "비지니스·경제·사회", Status = "Go" },
        };
        }

        private void LoadItems2()
        {
            Items = new List<Item>
        {
            new Item { Title = "1~20", Status = "Go" },
            new Item { Title = "21~40", Status = "Go" },
            new Item { Title = "41~60", Status = "Go" },
            new Item { Title = "61~80", Status = "Go" },
            new Item { Title = "81~100", Status = "Go" },
        };
        }


    }

}
