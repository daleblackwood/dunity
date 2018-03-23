using UnityEngine;
using System;
using System.Collections.Generic;

namespace Behave
{
    public enum BResult
    {
        Beginning,
        Ticking,
        Succeeded,
        Failed
    }

    public class BNode
    {
        static public bool logging = false;

        public string name { get; private set; }
        public BResult result { get; private set; }
        public float time { get; private set; }
        public float nodeTime { get; protected set; }
        public int nodeTicks { get; protected set; }
        public int ticks { get; private set; }
        public int runCount { get; private set; }
        protected float dt = 0.0f;

        public BNode(string name)
        {
            this.name = name;
            Init();
        }

        public virtual void Init()
        {
            result = BResult.Beginning;
            runCount++;
        }

        public void CallBegin()
        {
            time = 0.0f;
            ticks = 0;
            result = BResult.Ticking;
            OnBegin();
        }

        public BResult CallTick(float dt)
        {
            if (result > BResult.Ticking)
            {
                result = BResult.Beginning;
            }

            if (result == BResult.Beginning)
            {
                CallBegin();
            }

            ticks++;
            nodeTicks++;
            this.dt = dt;
            time += dt;
            nodeTime += dt;
            var tickResult = OnTick();

            if (result > BResult.Ticking)
            {
                Cancel();
                OnEnd();
            }

            result = tickResult;
            
            return result;
        }

        public virtual void Cancel()
        {
            if (result <= BResult.Ticking)
            {
                result = BResult.Failed;
            }
        }

        public void Restart()
        {
            Cancel();
            Init();
        }

        protected virtual void OnBegin() {}

        protected virtual BResult OnTick()
        {
            return BResult.Failed;
        }

        protected virtual void OnEnd() {}
    }

    public class BAction : BNode
    {
        static public readonly BAction SUCCEED = new BAction("BAction.SUCCEED", () => BResult.Succeeded);
        static public readonly BAction FAIL = new BAction("BAction.FAIL", () => BResult.Failed);

        Func<BResult> action = null;

        public BAction(string name, Func<BResult> action)
            : base(name)
        {
            if (action == null) 
            {
                throw new Exception("action can't be null.");
            }
            this.action = action;
        }

        protected override BResult OnTick()
        {
            var result = BResult.Failed;
            if (action != null)
            {
                result = action();
            }
            return result;
        }

        public override string ToString()
        {
            return this.GetType() + ": " + this.name;
        }
    }

    public class BActions : BNode
    {
        Action beginAction = null;
        Func<BResult> tickAction = null;
        Action endAction = null;

        public BActions(string name, Action beginAction, Func<BResult> tickAction, Action endAction = null)
            : base(name)
        {
            if (beginAction == null) 
            {
                throw new Exception("beginAction can't be null.");
            }
            if (tickAction == null) 
            {
                throw new Exception("tickAction can't be null.");
            }
            this.beginAction = beginAction;
            this.tickAction = tickAction;
            this.endAction = endAction;
        }

        protected override void OnBegin()
        {
            if (beginAction != null)
            {
                beginAction();
            }
        }

        protected override BResult OnTick()
        {
            var actionResult = BResult.Failed;

            if (tickAction != null)
            {
                actionResult = tickAction();
            }

            return actionResult;
        }

        protected override void OnEnd()
        {
            if (endAction != null)
            {
                endAction();
            }
        }
    }

    public abstract class BParent : BNode
    {
        protected BNode[] subnodes = null;
        protected int nodeIndex = 0;

        public BParent(string name, BNode[] subnodes)
            : base(name)
        {
            if (subnodes == null)
            {
                throw new Exception("Can't have null subnodes array");
            }
            if (subnodes.Length < 1)
            {
                throw new Exception("Can't have empty subnodes array");
            }
            this.subnodes = subnodes;
        }

        public override void Init()
        {
            nodeIndex = 0;
        }

        protected override void OnBegin()
        {
            nodeIndex = 0;
            foreach(var subnode in subnodes)
            {
                subnode.Init();
            }
        }

        protected override void OnEnd()
        {
            nodeIndex = 0;
            Cancel();
        }

        public override void Cancel()
        {
            base.Cancel();

            var children = GetChildren();
            if (children != null && children.Length > 0)
            {
                foreach (var child in children)
                {
                    child.Cancel();
                }
            }
        }
        
        public virtual BNode[] GetChildren()
        {
            return subnodes;
        }
        
        public override string ToString()
        {
            return this.GetType() + ": " + this.name + "(" + subnodes.Length + ")";
        }
    }

    public class BSequence : BParent
    {
        public BSequence(string name, BNode[] subnodes)
            : base(name, subnodes) {}

        protected override BResult OnTick()
        {
            if (nodeIndex >= subnodes.Length)
                nodeIndex = 0;

            var current = subnodes[nodeIndex];
            if (current != null)
            {
                var currentResult = current.CallTick(dt);

                if (currentResult == BResult.Succeeded) 
                {
                    nodeIndex++;
                    nodeTime = 0.0f;
                    nodeTicks++;
                    if (nodeIndex >= subnodes.Length)
                    {
                        return BResult.Succeeded;
                    }
                }
                else if (currentResult == BResult.Failed)
                {
                    return BResult.Failed; 
                }
            }

            return BResult.Ticking;
        }
    }

    public class BTandem : BParent
    {
        public BTandem(string name, BNode[] subnodes)
            : base(name, subnodes) {}

        protected override void OnBegin()
        {
            foreach(var subnode in subnodes)
            {
                subnode.CallBegin();
            }
        }

        protected override BResult OnTick()
        {
            var tickResult = BResult.Ticking;

            foreach (var subnode in subnodes)
            {
                var nodeResult = subnode.CallTick(dt);
                if (nodeResult > BResult.Ticking)
                {
                    tickResult = nodeResult;
                    break;
                }
            }

            return tickResult;
        }
    }

    public class BDriver : BParent
    {
        static public readonly BDriver EMPTY = new BDriver("BDriver.EMPTY", BAction.FAIL);

        public BNode defaultNode { get; private set; }
        public BNode currentNode { get; private set; }

        protected List<BNode> usedNodes = new List<BNode>();

        public BDriver(string name, BNode defaultNode)
            : base(name, new BNode[] { defaultNode }) 
        {
            this.defaultNode = defaultNode;
            SetNode(defaultNode);
        }

        override protected void OnBegin()
        {
            base.OnBegin();
            SetNode(defaultNode);
        }

        override protected BResult OnTick()
        {
            if (currentNode == null || currentNode.result > BResult.Ticking)
            {
                currentNode = defaultNode;
                currentNode.CallBegin();
            }

            return currentNode.CallTick(dt);
        }
        
        public void SetNode(BNode node)
        {
            nodeTime = 0.0f;
            nodeTicks = 0;
            currentNode = node;
            if (usedNodes.Contains(node) == false)
            {
                usedNodes.Add(node);
                subnodes = usedNodes.ToArray();
            }
        }

        override public BNode[] GetChildren()
        {
            if (subnodes.Length != usedNodes.Count)
            {
                subnodes = usedNodes.ToArray();
            }
            return subnodes;
        }
    }

    public class BToggle : BNode
    {
        BNode node;
        BNode successNode;
        BNode failNode;

        BNode currentNode;
        bool finished = false;

        public BToggle(string name, BNode node, BNode successNode, BNode failNode)
            : base(name)
        {
            this.node = node;
            this.successNode = successNode;
            this.failNode = failNode;
        }

        protected override void OnBegin()
        {
            currentNode = node;
            finished = false;
        }

        protected override BResult OnTick()
        {
            if (currentNode == null)
                return BResult.Failed;

            var nodeResult = currentNode.CallTick(dt);
            if (nodeResult == BResult.Ticking)
                return BResult.Ticking;

            if (finished == false)
            {
                if (result == BResult.Succeeded)
                {
                    currentNode = successNode;
                }
                else
                {
                    currentNode = failNode;
                }
                finished = true;
                nodeTime = 0;
                nodeTicks = 0;
            }

            return result;
        }
    }

    public class BTimed : BNode
    {
        BNode node = null;
        float minTime = 0.0f;
        float maxTime = 0.0f;
        float runTime = 0.0f;

        public BTimed(float time, BNode node)
            : base(node.name + "-timer")
        {
            this.node = node;
            this.minTime = this.maxTime = time;
        }

        public BTimed(float minTime, float maxTime, BNode node)
            : base(node.name + "-timer")
        {
            this.node = node;
            this.minTime = minTime;
            this.maxTime = maxTime;
        }

        protected override void OnBegin()
        {
            runTime = UnityEngine.Random.Range(minTime, maxTime);
        }

        protected override BResult OnTick()
        {
            var nodeResult = node.CallTick(dt);

            if (nodeResult >= BResult.Ticking)
                return nodeResult;

            if (time >= runTime)
                return BResult.Failed;

            return BResult.Ticking;
        }
    }
}
