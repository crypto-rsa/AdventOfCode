using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day13 : IAdventDay
    {
        private struct Field
        {
            public Field(int x, int y, char type)
            {
                X = x;
                Y = y;
                Type = type;
            }

            public int X { get; }

            public int Y { get; }

            public char Type { get; }
        }

        private class Cart
        {
            public Cart(int x, int y, char c, int index)
            {
                PositionX = x;
                PositionY = y;
                Index = index;

                switch (c)
                {
                    case '<':
                        VelocityX = -1;
                        break;

                    case '>':
                        VelocityX = +1;
                        break;

                    case '^':
                        VelocityY = -1;
                        break;

                    case 'v':
                        VelocityY = +1;
                        break;
                }
            }

            public int PositionX { get; private set; }

            public int PositionY { get; private set; }

            public int VelocityX { get; private set; }

            public int VelocityY { get; private set; }

            public int Intersections { get; private set; }

            public int Index { get; }

            public int CollisionIndex { get; internal set; } = -1;

            public void UpdatePosition()
            {
                PositionX += VelocityX;
                PositionY += VelocityY;
            }

            public void UpdateVelocity(Field field)
            {
                switch (field.Type)
                {
                    case '+':
                        switch (Intersections % 3)
                        {
                            case 0:
                                Turn(toRight: false);
                                break;
                            case 2:
                                Turn(toRight: true);
                                break;
                        }
                        Intersections++;
                        break;

                    case '/':
                        Turn(toRight: VelocityY != 0);
                        break;

                    case '\\':
                        Turn(toRight: VelocityX != 0);
                        break;

                    case '-':
                    case '|':
                        break;

                    default:
                        throw new ArgumentException("Invalid field!", nameof(field));
                }

                void Turn(bool toRight)
                {
                    if (VelocityY != 0)
                    {
                        VelocityX = (VelocityY > 0 ? +1 : -1) * (toRight ? -1 : +1);
                        VelocityY = 0;
                    }
                    else
                    {
                        VelocityY = (VelocityX > 0 ? -1 : +1) * (toRight ? -1 : +1);
                        VelocityX = 0;
                    }
                }
            }
        }

        private class Board
        {
            public Board()
            {
                Initialize(System.IO.File.ReadAllLines(@"2018/Resources/day13.txt"));
            }

            private void Initialize(string[] lines)
            {
                Fields = new Field[lines[0].Length, lines.Length];

                for (int row = 0; row < lines.Length; row++)
                {
                    var line = lines[row];
                    for (int col = 0; col < line.Length; col++)
                    {
                        char field;
                        Cart cart = null;
                        switch (line[col])
                        {
                            case '<':
                            case '>':
                                field = '-';
                                cart = new Cart(col, row, line[col], Carts.Count);
                                break;

                            case '^':
                            case 'v':
                                field = '|';
                                cart = new Cart(col, row, line[col], Carts.Count);
                                break;

                            default:
                                field = line[col];
                                break;
                        }

                        Fields[col, row] = new Field(col, row, field);

                        if (cart != null)
                        {
                            Carts.Add(cart);
                        }
                    }
                }
            }

            public HashSet<Cart> Step()
            {
                var collidedCarts = new HashSet<Cart>();
                foreach (var cart in Carts.OrderBy(c => c.PositionY).ThenBy(c => c.PositionX))
                {
                    if (collidedCarts.Contains(cart))
                        continue;

                    cart.UpdatePosition();
                    cart.UpdateVelocity(Fields[cart.PositionX, cart.PositionY]);

                    foreach (var c in Carts.GroupBy(c => (X: c.PositionX, Y: c.PositionY)).Where(g => g.Count() > 1).SelectMany(g => g))
                    {
                        collidedCarts.Add(c);
                    }
                }

                Iterations++;

                // Save();

                return collidedCarts;

                void Save()
                {
                    int columns = Fields.GetLength(0);
                    int rows = Fields.GetLength(1);

                    var lines = Enumerable.Range(0, rows).Select(r => new string(Enumerable.Range(0, columns).Select(c => GetChar(c, r)).ToArray())).ToArray();

                    System.IO.File.WriteAllLines($@"2018/Day13/board{Iterations}.txt", lines);

                    char GetChar(int x, int y)
                    {
                        var carts = Carts.Where(c => c.PositionX == x && c.PositionY == y).ToList();
                        switch (carts.Count)
                        {
                            case 0: return Fields[x, y].Type;
                            case 1: return (char)('A' + carts.First().Index);
                            default: return '*';
                        }

                    }
                }
            }

            public Field[,] Fields { get; private set; }

            public List<Cart> Carts { get; } = new List<Cart>();

            public int Iterations { get; private set; }
        }

        public string Name => "13. 12. 2018";

        public string Solve()
        {
            var board = new Board();

            while (true)
            {
                var collidedCarts = board.Step();
                var cart = collidedCarts.OrderBy(c => c.PositionY).ThenBy(c => c.PositionX).FirstOrDefault();

                if (cart != null)
                    return $"{cart.PositionX},{cart.PositionY} (in {board.Iterations} iterations)";

            }
        }

        public string SolveAdvanced()
        {
            var board = new Board();

            while (true)
            {
                var collidedCarts = board.Step();
                board.Carts.RemoveAll(c => collidedCarts.Contains(c));
                if (board.Carts.Count == 1)
                {
                    var cart = board.Carts.Single();

                    return $"{cart.PositionX},{cart.PositionY} (in {board.Iterations} iterations)";
                }
            }
        }
    }
}