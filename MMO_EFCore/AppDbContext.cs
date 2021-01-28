using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMO_EFCore
{
    // EF Core 작동 스탭
    // 1) DbContext 만들 때
    // 2) DBSet<T>를 찾는다
    // 3) 모델링 class 분석해서 칼럼을 찾는다.
    // 4) 모델링 class에서 참조하는 다른 class가 있으면 걔도 분석,
    // 5) OnModelCreating 함수 호출 (추가 설정 = override)
    // 6) 데이터 베이스의 전체 모델링 구조를 내부 메모리에 들고 있음
    public class AppDbContext : DbContext
    {
        // DBSet<Item> -> ef core한테 알려준다
        // item이라는 db테이블이 있는데, 세부적인 칼럼/키 정보는 item 클래스에 참조해라.
        public DbSet<Item> items { get; set; }

        // DB ConnectionString
        // 어떤 애들을 어떻게 연결해라. (각종 설정, Auth)
        public const string ConnectionString = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=EFCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionString);
        }
    }
}
