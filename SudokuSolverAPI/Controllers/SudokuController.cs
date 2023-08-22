using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SudokuSolverAPI.Models;

namespace SudokuSolverAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SudokuController : ControllerBase
    {
        Stack<Node> path=new Stack<Node>();
        private List<List<Node>> Table;
        [HttpPost("Start")]
        public async Task<IActionResult> Start()
        {
            Table = new List<List<Node>>{
         new List<Node> { new Node(), new Node(),new Node { CurrentValue=6},new Node(),new Node(),new Node(),new Node { CurrentValue=5},new Node(),new Node { CurrentValue=8} },
         new List<Node> { new Node { CurrentValue=1}, new Node(),new Node { CurrentValue=2},new Node { CurrentValue = 3 }, new Node { CurrentValue = 8 }, new Node(),new Node(),new Node(),new Node { CurrentValue=4} },
         new List<Node> { new Node(), new Node(),new Node(),new Node { CurrentValue = 2 }, new Node(),new Node(),new Node { CurrentValue = 1 }, new Node { CurrentValue = 9 }, new Node() },
         new List<Node> { new Node(), new Node(),new Node(),new Node(),new Node { CurrentValue = 6 }, new Node { CurrentValue = 3 }, new Node(),new Node { CurrentValue = 4 }, new Node { CurrentValue=5} },
         new List<Node> { new Node(), new Node { CurrentValue = 6 }, new Node { CurrentValue=3},new Node { CurrentValue = 4 }, new Node(),new Node { CurrentValue = 5 }, new Node { CurrentValue=8},new Node { CurrentValue = 7 }, new Node() },
         new List<Node> { new Node { CurrentValue = 5 }, new Node { CurrentValue = 4 }, new Node(),new Node { CurrentValue = 9 }, new Node { CurrentValue = 2 }, new Node(),new Node(),new Node(),new Node() },
         new List<Node> { new Node(), new Node { CurrentValue = 8 }, new Node { CurrentValue=7},new Node(),new Node(),new Node { CurrentValue = 4 }, new Node(),new Node(),new Node() },
         new List<Node> { new Node { CurrentValue = 2 }, new Node(),new Node(),new Node(),new Node { CurrentValue = 9 }, new Node { CurrentValue = 8 }, new Node { CurrentValue=4},new Node(),new Node { CurrentValue=7} },
         new List<Node> { new Node { CurrentValue = 4 }, new Node(),new Node { CurrentValue=9},new Node(),new Node(),new Node(),new Node { CurrentValue = 3 }, new Node(),new Node() }
};
            Node startnode = FindNextPosition();
            FindAllPosibleNumbers(startnode);
            Solver(startnode);
            return Ok(Table);
        }
        private Node FindNextPosition()
        {
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int count = 0;
                    if (Table[i][j].CurrentValue == null)
                    {
                        for (int k = 0; k < 9; k++)
                        {
                            if (Table[i][k].CurrentValue != null)
                                count++;
                        }
                        for (int l = 0; l < 9; l++)
                        {
                            if (Table[l][j].CurrentValue != null)
                                count++;
                        }
                        Table[i][j].KnownNumberCount = count;
                        Table[i][j].Y = i + 1;
                        Table[i][j].X = j + 1;
                        nodes.Add(Table[i][j]);
                    }

                }
            }
            nodes.Sort(delegate (Node x, Node y)
            {
                return x.KnownNumberCount - y.KnownNumberCount;
            });
            if (nodes.Count == 0)
                return null;
            return nodes[nodes.Count - 1];
        }
        private void FindAllPosibleNumbers(Node node)
        {
            List<int> number = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //tim nhung so con lai cua column
            List<int> row = new List<int>();
            List<int> col = new List<int>();
            List<int> square3x3 = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (Table[node.Y - 1][i].Possibles == null&& Table[node.Y - 1][i].CurrentValue!=null)
                    row.Add((int)Table[node.Y - 1][i].CurrentValue);
                if (Table[i][node.X - 1].Possibles == null&& Table[i][node.X - 1].CurrentValue!=null)
                    col.Add((int)Table[i][node.X - 1].CurrentValue);
            }
            int[] zone = FindZone(node);
            //lam lai tinh so trong zone
            for (int i = (zone[1]-1)*3; i < zone[1]*3; i++)
            {
                for (int j = (zone[0]-1)*3; j < zone[0]*3; j++)
                {
                    if (Table[i][j].Possibles == null&& Table[i][j].CurrentValue!=null)
                        square3x3.Add((int)Table[i][j].CurrentValue);
                }
            }
            IEnumerable<int> Posible = number.Except(square3x3).Intersect(number.Except(row).Intersect(number.Except(col)));
            node.Possibles = new Dictionary<int, bool>();
            foreach (int i in Posible)
            {
                node.Possibles.Add(i, true);
            }
        }
        private int[] FindZone(Node node)
        {
            return new int[2] { ((node.X - 1) / 3 + 1), ((node.Y - 1) / 3 + 1) };
        }
        private void Solver(Node nextnode)
        {
            while (Recursion(nextnode) != false)
            {
                nextnode = FindNextPosition();
                if (nextnode == null)
                    break;
                FindAllPosibleNumbers(nextnode);
            }
        }
        private bool Recursion(Node node)
        {
            var possible = from number in node.Possibles.Keys
                           where node.Possibles[number] == true
                           select number;
            if (possible.Count() == 0)
            {
                Node previous = path.Pop();
                previous.Possibles[(int)previous.CurrentValue] = false;
                Console.WriteLine($"xoa {node.X}|{node.Y}");
                Solver(previous);
            }
            node.CurrentValue = possible.First();
            path.Push(node);
            Console.WriteLine($"nhap {node.X}|{node.Y}:{node.CurrentValue}");
            return true;
        }
    }
}
