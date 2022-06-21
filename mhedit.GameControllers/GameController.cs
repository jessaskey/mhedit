using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.GameControllers
{
    public class GameController
    {
        protected string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.!-,%:o()hs@><?c";

        public enum SourceFile
        {
            Page6,
            Page7,
            Token,
            Cannon,
            MazeMessages
        }
    }
}
