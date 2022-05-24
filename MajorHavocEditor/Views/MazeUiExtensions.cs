using System.Linq;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{

    public static class MazeUiExtensions
    {
        public static void Show(this IWindowManager manager, Maze maze, bool create = false )
        {
            MazeUi mazeUi = (MazeUi)
                            manager.Interfaces.FirstOrDefault(
                                ui => ui is MazeUi mui && mui.Maze.Equals(maze)) ??
                            (create ? new MazeUi( maze ) : null);

            if (mazeUi != null)
            {
                manager.Show(mazeUi);
            }
        }

        public static void Remove(this IWindowManager manager, Maze maze)
        {
            manager.Remove( (MazeUi)
                manager.Interfaces.FirstOrDefault(
                    ui => ui is MazeUi mui && mui.Maze.Equals( maze ) ) );
        }
    }

}