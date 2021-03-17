using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PropertyChanged;
using System.Windows;
using System.Collections;
using System.Windows.Threading;
using System.Threading;

namespace WpfApp_8puzzles
{
    /// <summary>8 Puzzles實作 (Fody Embedded)</summary>
    public class Puzzles : BaseViewModel
    {
        public const int DefaultValue = 123804765;
        public const int PWidth = 3;
        public const int PHeight = 3;
        public const int PSize = PWidth * PHeight;
        public static string strF = string.Format("D{0}", PSize);
#region Properties
        private UserControl_Puzzles puzzzleUI;
        private int value_init;
        private int value_trgt;
        private int value_sync;

        public int ValueSync {
            get { return value_sync; }
            set
            {
                value_sync = value;
                SyncWith(puzzzleUI);
            }
        }
        public void SyncWith(UserControl_Puzzles PuzzzleUI)
        {
            this.puzzzleUI = PuzzzleUI;
            if (puzzzleUI != null)
                puzzzleUI.PuzzleData = value_sync;
        }
        public int InitValue {
            get { return value_init; }
            set
            {
                if(!CheckValid(value))
                {
                    MessageBox.Show("不合法的資料");
                    return;
                }
                value_init = value;
                ValueSync = value_init;
            }
        }
        public int TargetValue {
            get { return value_trgt; }
            set
            {
                if(!CheckValid(value))
                {
                    MessageBox.Show("不合法的資料");
                    return;
                }
                value_trgt = value;
            }
        }
        public int Degree { get; set; }
        public List<int> BFSStack { get; set; }
        public List<int> BFSTracker { get; set; }
        public List<int> AnserStack { get; set; }
        
        public string AnswerStr
        {
            get { return Cost.ToString() + (AnswerFound ? "(有解)" : "(無解)"); }
            set { }
        }
        public string TimeCostStr {
            get { return timeCost.ToString("F4"); }
            set { }
        }
        public double timeCost { get; set; }
        public int Cost { get; private set; }
        public bool AnswerFound { get; private set; }
#endregion
        
        /// <summary>建構式 </summary>
        public Puzzles()
        {
            BFSTracker = new List<int>();
            BFSStack = new List<int>();
            InitValue = DefaultValue;
            ValueSync = InitValue;
            TargetValue = DefaultValue;
        }

        /// <summary>檢查資料是否正確無誤 </summary>
        public static bool CheckValid(int Data)
        {
            string data = Data.ToString(strF);
            if (data.Length != PSize)
                return false;
            for (int i = 0; i < PSize; i++)
                if (!data.Contains(i.ToString()))
                    return false;
            return true;
        }
        /// <summary>BFS 8Puzzle</summary>
        public void BFS_Search(bool? EnableUI = true)
        {
            ThreadPool.QueueUserWorkItem(o => /// https://dotblogs.com.tw/yc421206/2011/10/10/40840
            {
                ///清資料
                DateTime StartTime = DateTime.Now;
                BFSStack.Clear();
                BFSTracker.Clear();
                AnswerFound = false;
                Cost = 0;
                timeCost = 0;
                Costs.Clear();
                Parent.Clear();
                ///初始化
                BFSStack.Add(InitValue);
                BFSTracker.Add(InitValue);
                Parent.Add(InitValue, InitValue);
                BFSTracker.Add(0);
                Costs.Add(InitValue, 0);
                int cost = 0, bfs0 = 0;

                while (!AnswerFound && BFSStack.Count > 0)
                {
                    bfs0 = BFSStack[0];
                    cost = (int)Costs[bfs0];
                    //介面同步
                    if(EnableUI ?? false)
                    {
                        puzzzleUI.Dispatcher.Invoke( 
                            (Action)delegate {
                                ValueSync = bfs0;
                                Cost = cost;
                                timeCost = (DateTime.Now - StartTime).TotalMilliseconds;
                                Degree = Costs.Count;
                            }
                        );
                    }
                    AddStates(bfs0, cost);
                }
                DateTime FinishTime = DateTime.Now;
                timeCost = (FinishTime - StartTime).TotalMilliseconds;
                Degree = Costs.Count;
                Cost = cost;
            });
        }

        /// <summary>以Parent查解的方法</summary>
        public void DoAnswer()
        {
            //介面同步
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (AnswerFound)
                {
                    AnserStack = new List<int>();
                    AnserStack.Add(TargetValue);
                    int parent = (int)Parent[TargetValue];
                    while (parent!=InitValue)
                    {
                        AnserStack.Add(parent);
                        parent = (int)Parent[parent];
                    }
                    AnserStack.Add(InitValue);

                    for (int i = AnserStack.Count-1; i >=0; --i)
                    {
                        puzzzleUI.Dispatcher.Invoke(
                            (Action)delegate
                            {
                                ValueSync = AnserStack[i];
                                Cost = AnserStack.Count - i - 1;
                            }
                        );
                        Thread.Sleep(500);
                    }
                }
            });
        }
        private List<int> Statesbuffer = new List<int>();
        private Hashtable Costs   = new Hashtable();
        private Hashtable Parent  = new Hashtable();
        /// <summary>BFS搜尋查找子狀態新增至List</summary>
        private void AddStates(int nowValue, int Cost)
        {
            if (nowValue == TargetValue)
            {
                BFSTracker.Add(0);//劃分用
                BFSStack.Remove(nowValue);
                AnswerFound = true;
                return;
            }

            int IDof0 = FindPos1D(nowValue, 0);
            if (IDof0 < 0) return;
            Statesbuffer.Clear();
            // W   3   W
            // --------- H
            // | 0 1 2 |
            // | 3 4 5 | 3
            // | 6 7 8 |
            // --------- H

            //可向右換
            if (!((IDof0 - (PWidth - 1)) % 3 == 0))
                Statesbuffer.Add(ValueOfExchange(nowValue, IDof0, IDof0 + 1));
            //可向左換
            if (!(IDof0 % PWidth == 0))
                Statesbuffer.Add(ValueOfExchange(nowValue, IDof0, IDof0 - 1));
            //可向上換
            if (!(IDof0 < PWidth))
                Statesbuffer.Add(ValueOfExchange(nowValue, IDof0, IDof0 - 3));
            //可向下換
            if (!(IDof0 + PWidth > PSize -1))
                Statesbuffer.Add(ValueOfExchange(nowValue, IDof0, IDof0 + 3));
            
           //新增到陣列中
            foreach (int value in Statesbuffer)
                if(value > 0 && !Costs.ContainsKey(value))
                {
                    BFSTracker.Add(value);
                    BFSStack.Add(value);
                    Costs.Add(value, Cost + 1);
                    Parent.Add(value, nowValue);
                }
            BFSTracker.Add(0);//劃分用
            BFSStack.Remove(nowValue);
        }
        /// <summary>數字位於第幾個位置</summary>
        public static int FindPos1D(int Value,int TargetID)
        {
            char c = '\0'; char.TryParse(TargetID.ToString(),out c);
            if(c != '\0')
                return Value.ToString(strF).IndexOf(c); ;
            return -1;
        }
        /// <summary>位置交換後的值</summary>
        public static int ValueOfExchange(int nowValue, int Pos0, int Pos1)
        {
            if (Pos0 < 0 || Pos0 >= PSize || 
                Pos1 < 0 || Pos1 >= PSize)
                return -1;
            StringBuilder strV = new StringBuilder (nowValue.ToString(strF));
            char ctemp = strV[Pos0];
            ctemp = strV[Pos0];
            strV[Pos0] = strV[Pos1];
            strV[Pos1] = ctemp;
            int v = -1; int.TryParse(strV.ToString(), out v);
            return v;
        }
    }
    
    /// <summary> 用Fody減少Coding需求 的基礎型別 </summary>
    [AddINotifyPropertyChangedInterfaceAttribute]
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = null;
    }
}
