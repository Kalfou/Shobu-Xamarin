using Shobu.Persistence;
using System;
using System.Threading.Tasks;

namespace Shobu.Model
{
    public class ShobuModel
    {
        #region Fields

        private readonly IShobuDataAccess _access;
        private ShobuState _state;

        private int _stepState;

        private int pb, px, py, pdx, pdy;
        private int vx, vy, vm;
        private int ab, ax, ay;

        #endregion

        #region Properties

        public int this[int x, int y]
        {
            get
            {
                int b = 0;
                TransformIndexes(ref b, ref x, ref y);

                return _state[b, x, y];
            }
        }

        public int FieldState(int x, int y)
        {
            if (x < 0 || x >= 8)
            {
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            }

            if (y < 0 || y >= 8)
            {
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
            }

            int b = 0;
            TransformIndexes(ref b, ref x, ref y);

            switch (_stepState)
            {
                case 0:
                    if (IsLegalPassive(b, x, y))
                    {
                        return 3;
                    }
                    break;
                case 1:
                    if (b == pb && x == px && y == py)
                    {
                        return 4;
                    }
                    else if (IsLegalDestination(b, x, y))
                    {
                        return 3;
                    }
                    break;
                case 2:
                    if (b == pb && x == px && y == py)
                    {
                        return 4;
                    }

                    if (b == pb && x == pdx && y == pdy)
                    {
                        return 4;
                    }

                    else if (IsLegalAgressive(b, x, y))
                    {
                        return 3;
                    }
                    break;
                default:
                    break;
            }

            return b % 2 == 1 ? 2 : 1;
        }

        #endregion

        #region Events

        public event EventHandler<ShobuEventArgs> GameChanged;

        public event EventHandler<ShobuEventArgs> StepGame;

        #endregion

        #region Constructors
        public ShobuModel(IShobuDataAccess access)
        {
            _state = new ShobuState();
            _access = access;

            NewGame();
        }

        #endregion

        #region Public Methods

        public void NewGame()
        {
            _state.Turn = 1;
            _stepState = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        _state[i, j, k] = 0;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _state[i, 0, j] = 2;
                    _state[i, 3, j] = 1;

                }
            }

            StepGame?.Invoke(this, new ShobuEventArgs(0));
        }

        public void FieldClick(int x, int y)
        {
            int b = 0;
            TransformIndexes(ref b, ref x, ref y);

            switch (_stepState)
            {
                case 0:

                    if (IsLegalPassive(b, x, y))
                    {
                        px = x;
                        py = y;
                        pb = b;

                        _stepState++;

                        GameChanged?.Invoke(this, new ShobuEventArgs(0));
                    }
                    break;

                case 1:

                    int tx, ty, tm;

                    if (b == pb)
                    {
                        tx = Math.Abs(x - px);
                        ty = Math.Abs(y - py);

                        if ((tx == ty && tx != 0) || (tx * ty == 0 && tx + ty != 0))
                        {
                            tm = Math.Max(tx, ty);

                            tx = (x - px) / tm;
                            ty = (y - py) / tm;

                            if (IsLegalDirection(tx, ty, tm))
                            {
                                pdx = x;
                                pdy = y;

                                vx = tx;
                                vy = ty;
                                vm = tm;

                                _stepState++;

                                GameChanged?.Invoke(this, new ShobuEventArgs(0));
                            }
                        }
                    }
                    break;

                case 2:

                    if (IsLegalAgressive(b, x, y))
                    {
                        ab = b;
                        ax = x;
                        ay = y;

                        ApplyMove();
                        EndTurn();
                        _stepState = 0;

                        StepGame?.Invoke(this, new ShobuEventArgs(GetVictor()));
                    }
                    break;

                default:
                    break;
            }
        }

        public void ResetStep()
        {
            _stepState = 0;
        }

        public async Task LoadGameAsync(string name)
        {
            if (_access == null)
            {
                throw new InvalidOperationException("No data access is provided.");
            }

            _state = await _access.LoadAsync(name);

            _stepState = 0;

            StepGame?.Invoke(this, new ShobuEventArgs(0));
        }

        public async Task SaveGameAsync(string name)
        {
            if (_access == null)
            {
                throw new InvalidOperationException("No data access is provided.");
            }

            await _access.SaveAsync(name, _state);
        }

        #endregion

        #region Private methods

        private void ApplyMove()
        {
            _state[pb, px, py] = 0;
            _state[pb, pdx, pdy] = _state.Turn;


            bool hasPush = false;
            for (int i = 1; i <= vm; i++)
            {
                if (_state[ab, ax + (vx * i), ay + (vy * i)] == GetOpponent())
                {
                    hasPush = true;
                    _state[ab, ax + (vx * i), ay + (vy * i)] = 0;
                }

            }

            if (hasPush && IsOnBoard(ax + (vx * (vm + 1)), ay + (vy * (vm + 1))))
            {
                _state[ab, ax + (vx * (vm + 1)), ay + (vy * (vm + 1))] = GetOpponent();
            }

            _state[ab, ax, ay] = 0;
            _state[ab, ax + (vx * vm), ay + (vy * vm)] = _state.Turn;
        }

        private bool IsLegalPassive(int b, int x, int y)
        {
            return IsHomeBoard(_state.Turn, b) &&
                   IsOnBoard(x, y) &&
                   _state[b, x, y] == _state.Turn;
        }

        private bool IsLegalDestination(int b, int x, int y)
        {
            int tx, ty, tm;

            if (b == pb)
            {
                tx = Math.Abs(x - px);
                ty = Math.Abs(y - py);

                if ((tx == ty && tx != 0) || (tx * ty == 0 && tx + ty != 0))
                {
                    tm = Math.Max(tx, ty);

                    tx = (x - px) / tm;
                    ty = (y - py) / tm;

                    bool ret = IsLegalDirection(tx, ty, tm);

                    return ret;
                }
            }
            return false;
        }

        private bool IsLegalDirection(int x, int y, int m)
        {
            if (m != 1 && m != 2)
            {
                return false;
            }

            if (x > 1 || x < -1)
            {
                return false;
            }

            if (y > 1 || y < -1)
            {
                return false;
            }

            if (x == 0 && y == 0)
            {
                return false;
            }

            if (!IsOnBoard(px + (x * m), py + (y * m)))
            {
                return false;
            }

            for (int i = 1; i <= m; i++)
            {
                if (_state[pb, px + (x * i), py + (y * i)] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsLegalAgressive(int b, int x, int y)
        {
            if (b < 0 || b > 3 || !IsOnBoard(x,y))
            {
                return false;
            }

            if (pb % 2 == b % 2)
            {
                return false;
            }

            if (_state[b, x, y] != _state.Turn)
            {
                return false;
            }

            if (!IsOnBoard(x + (vx * vm), y + (vy * vm)))
            {
                return false;
            }

            int hasPush = 0;
            for (int i = 1; i <= vm + hasPush; i++)
            {
                if (IsOnBoard(x + (vx * i), y + (vy * i)))
                {
                    int tv = _state[b, x + (vx * i), y + (vy * i)];
                    if (tv == _state.Turn)
                    {
                        return false;
                    }
                    else if (tv == GetOpponent())
                    {
                        hasPush++;
                        if (hasPush > 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void EndTurn()
        {
            _state.Turn = GetOpponent();
        }

        private int GetVictor()
        {
            for (int i = 0; i < 4; i++)
            {
                int whites = 0;
                int blacks = 0;

                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        switch (_state[i,j,k])
                        {
                            case 1:
                                whites++;
                                break;
                            case 2:
                                blacks++;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (whites == 0)
                {
                    return 2;
                }

                if (blacks == 0)
                {
                    return 1;
                }
            }

            return 0;
        }

        private int GetOpponent()
        {
            return _state.Turn == 1 ? 2 : 1;
        }

        private bool IsOnBoard(int x, int y)
        {
            return x >= 0 && x < 4 && y >= 0 && y < 4;
        }

        private bool IsHomeBoard(int side, int boardId)
        {
            if (side == 2 && boardId >= 0 && boardId < 2)
            {
                return true;
            }
            if (side == 1 && boardId >= 2 && boardId < 4)
            {
                return true;
            }
            return false;
        }

        private void TransformIndexes(ref int b, ref int x, ref int y)
        {
            if (x >= 4)
            {
                x %= 4;
                b += 2;
            }
            if (y >= 4)
            {
                y %= 4;
                b += 1;
            }
        }

        #endregion
    }
}
