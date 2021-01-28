using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // 클래스 이름 = 테이블 이름 = Item
    // [Table("Item")]
    public class Item
    {
        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; set; }
        public int OwnerId { get; set; }
        // 다른 크래스 참조 -> FK (Navigational Property)
        public Player Owner { get; set; }
    }

    // 클래스 이름 = 테이블 = Player
    public class Player
    {
        // 이름 id -> pk
        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}
