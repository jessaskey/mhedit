using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit
{
    public static class GameProfileFactory
    {
        public static List<GameProfile> GetAvailableGameProfiles()
        {
            List<GameProfile> gameProfiles = new List<GameProfile>();
            gameProfiles.Add(GetMajorHavocProfile());
            gameProfiles.Add(GetMajorHavocPEProfile());
            return gameProfiles;
        }

        private static GameProfile GetMajorHavocProfile()
        {
            GameProfile gp = new GameProfile();
            gp.Name = "Major Havoc";

            return gp;
        }

        private static GameProfile GetMajorHavocPEProfile()
        {
            GameProfile gp = new GameProfile();
            gp.Name = "Major Havoc - The Promised End";

            return gp;
        }
    }
}
