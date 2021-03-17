using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using System.Windows.Threading;
using System.Threading;

namespace WpfApp_8puzzles
{
    /// <summary>8 Puzzles實作 (Fody Embedded)</summary>
    public class Puzzles
    {
        public static List<int> DefaultValue = new List<int>()
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,0
        };
        public static ulong TargetValue = State2Hash(DefaultValue);
        public double TimeOut = 60 * 1000; // ms
        public readonly Hashtable Parent = new Hashtable();
        public const int PWidth = 4;
        public const int PHeight = 4;
        public const int PSize = PWidth * PHeight;
        #region Properties
        private ulong InitValue;
        public List<ulong> Route = new List<ulong>();

        public int Degree { get; set; }
        public Queue<ulong> BFSQueue { get; set; }
        public double TimeCost { get; set; }
        public int CurrentDegree = 0;
        public bool AnswerFound = false;
        public static void Hash2State(ulong hashValue,ref List<int>LState)
        {
            if (LState == null) return;
            LState.Clear();
            for (int i = 0; i < PSize; i++)
            {
                ulong div = hashValue / 16;
                //mod value
                int mod = (int)(hashValue - div * 16);
                hashValue -= (ulong)mod;
                hashValue /= 16;
                LState.Add(mod);
            }
            return;
        }
        public static ulong State2Hash(List<int> state)
        {
            ulong value = 0; ulong pow = 1;
            for (int i = 0; i < PSize; i++)
            {
                value += ((ulong)state[i] * pow);
                pow *= 16;
            }
            return value;
        }
        /// <summary>計算下一個可能的位置hash</summary>
        public static List<ulong> NextPossibleState(ulong state)
        {
            List<ulong> NextStates = new List<ulong>();
            List<int> LState = new List<int>();
            Hash2State(state, ref LState);
            for (int i = 0; i < PSize; i++)
            {
                ///  0  1  2  3
                ///  4  5  6  7
                ///  8  9 10 11
                /// 12 13 14 15
                if(LState[i] == 0)
                {
                    if ((i + 1) % PWidth != 0) //0可以往右換 (非位置 3,7,11,15)
                    {
                        Exchage(ref LState, i, i + 1);
                        NextStates.Add(State2Hash(LState));
                        Exchage(ref LState, i, i + 1); // 回復
                    }
                    if (i % PWidth != 0) // 0可以往左換(非位置 3,7,11,15)
                    {
                        Exchage(ref LState, i, i - 1);
                        NextStates.Add(State2Hash(LState));
                        Exchage(ref LState, i, i - 1); // 回復
                    }
                    if (i >= PWidth) //0可以往上換(非位置 0,1,2,3)
                    {
                        Exchage(ref LState, i, i - PWidth);
                        NextStates.Add(State2Hash(LState));
                        Exchage(ref LState, i, i - PWidth); // 回復
                    }
                    if (i < PSize - PWidth) //0可以往下換(非位置 12,13,14,15)
                    {
                        Exchage(ref LState, i, i + PWidth);
                        NextStates.Add(State2Hash(LState));
                    }
                    return NextStates;
                }
            }
            return null;
        }
        private static void Exchage(ref List<int> LState,int i,int j)
        {
            int temp = LState[i];
            LState[i] = LState[j];
            LState[j] = temp;
        }
#endregion
        
        /// <summary>建構式 </summary>
        public Puzzles(UserControl_Puzzles puzzzleUI)
        {
            BFSQueue = new Queue<ulong>();
        }

        public void UpdateInitValueFromUI(UserControl_Puzzles puzzzleUI)
        {
            InitValue = State2Hash(puzzzleUI.GetCurrentPuzzles());
        }

        /// <summary>BFS Puzzle</summary>
        public void BFS_Search()
        {
            ThreadPool.QueueUserWorkItem(o => /// https://dotblogs.com.tw/yc421206/2011/10/10/40840
            {
                AnswerFound = false;
                ///清資料
                DateTime StartTime = DateTime.Now;
                BFSQueue.Clear();
                TimeCost = 0;
                Parent.Clear();
                ///初始化
                BFSQueue.Enqueue(InitValue);
                Parent.Add(InitValue, InitValue);
                Degree = -1;
                CurrentDegree = 0;
                int count = 0;
                while (!AnswerFound && BFSQueue.Count > 0)
                {
                    ulong bfsTop = BFSQueue.Dequeue();

                    CurrentDegree = Cost(bfsTop, out _);
                    if (bfsTop == TargetValue)
                    {
                        AnswerFound = true;
                        BFSQueue.Clear();
                        break;
                    }
                    foreach (ulong next in NextPossibleState(bfsTop))
                    {
                        if(!Parent.Contains(next))
                        {
                            BFSQueue.Enqueue(next);
                            Parent.Add(next, bfsTop);
                        }
                    }
                    count++;
                    TimeCost = (DateTime.Now - StartTime).TotalMilliseconds;
                }
                DateTime FinishTime = DateTime.Now;
                TimeCost = (FinishTime - StartTime).TotalMilliseconds;
                Degree = Cost(TargetValue,out Route); // 計算最終狀態到初始狀態花的cost
            });
        }
        /// <summary> 查找當前節點到樹根的路徑圖(解法)及花費的深度 </summary>
        public int Cost(ulong state, out List<ulong> Route)
        {
            int cost = 0;
            Route = new List<ulong>() { state };
            while (Parent.ContainsKey(state))
            {
                ulong parent = (ulong)Parent[state];
                if (parent == state)
                {
                    Route.Reverse();
                    return cost;
                }
                Route.Add(parent);
                cost++;
                state = parent;
            }
            return -1;
        }
    }
}
