using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
    // Migration
    // DB?

    // 일단 EF Core DbContext <-> DB 상태에 대해 동의가 있어야 함
    // 무엇을 기준으로 할 것인가? 닭이 먼저일까? 알이 먼저 일까?

    // 1) Code-First
    // - 지금까지 우리가 사용하던 방식 (Entity Class / DbContext가 기준)
    // - 항상 최신 상태로 DB를 업데이트 하고 싶다는 말은 아님

    // ++ Migration step ++
    // A) Migration 만들고
    // B) Migration 적용.

    // A) Add-Migration [Name]
    // - 1) DbContext를 찾아서 분석 -> DB 모델링 (최신)
    // - 2) ~ModelSnapshot.cs을 이용해서 가장 마지막 Migration 상태의 DB 모델링 (가장 마지막 상태)
    // - 3) 1-2 비교 결과 도출
    // -- a) ModelSnapshot -> 최신 db 모델링
    // -- b) Migrate.Designer.cs와 Migrate.cs -> Migration 관련된 세부 정보
    // 일단은 수동으로 Up/Down을 추가해도 됨.

    // B) Migration 적용
    // - 1) SQL change script
    // -- Script-Migration [From] [To] [Options]
    // - 2) Database.Migrate 호출
    // - 3) Command Line 방식
    // - Update-Database [options]

    // 특정 Migration으로 Sync (Update-Database [Name])
    
    // 마지막 Migration 삭제 (Remove-Migration) 

    // 2) Database-First

    // 3) SQL-First

    [Table("Item")]
    public class Item
    {
        public bool SoftDeleted { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; private set; }

        public int ItemGrade { get; set; }

        // 다른 클래스 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }        
        public Player Owner { get; set; }

    }


    // 클래스 이름 = 테이블 = Player
    [Table("Player")]
    public class Player
    {
        // 이름 id -> pk
        public int PlayerId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public Item OwnedItem { get; set; } // 1 : 다
        public Guild Guild; 

    }

    [Table("Guild")]
    public class Guild
    {
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Memebers { get; set; }
    }

    // DTO (Data Transfer Object)
    public class GuildDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
}
