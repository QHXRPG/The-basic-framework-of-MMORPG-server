using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    // 玩家信息
    [Table(Name ="player")]
    public class DbPlayer
    {
        [Column(IsIdentity = true, IsPrimary = true)]

        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Coin { get; set; }
    }

    // 玩家角色信息
    [Table(Name = "Character")]
    public class DbCharacter
    {
        [Column(IsIdentity = true, IsPrimary = true)]

        public int Id { get; set; }

        public int JobId { get; set; }

        public string Name { get; set; }

        public int Hp { get; set; } = 100;

        public int Mp { get; set; } = 100;

        public int Level { get; set; } = 1;

        public int Exp { get; set; } = 0;

        public int SpaceId { get; set; }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int Z { get; set; } = 0;

        public long Gold { get; set; } = 0;

        public int PlayerId { get; set; }
    }

}
