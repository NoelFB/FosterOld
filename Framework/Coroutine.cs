using System;
using System.Collections;
using System.Collections.Generic;

namespace Foster.Framework
{
    public class Coroutine
    {
        private readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        private float delay = 0f;
        private bool ended;

        public bool Finished { get; private set; }
        public Action? OnEnd;

        public Coroutine() 
        { 
        
        }
        
        public Coroutine(IEnumerator enumerator)
        {
            Start(enumerator);
        }

        public Coroutine Start(IEnumerator enumerator)
        {
            delay = 0f;
            stack.Clear();
            stack.Push(enumerator);
            Finished = false;
            return this;
        }

        public void Stop()
        {
            stack.Clear();
            ended = true;
            Finished = true;
        }

        public void Update()
        {
            if (delay > 0)
            {
                delay -= Time.Delta;
                return;
            }

            Step();
        }

        public void Step()
        {
            if (stack.Count > 0)
            {
                ended = false;

                var top = stack.Peek();
                if (top.MoveNext() && !ended)
                {
                    var value = top.Current;
                    if (value is float || value is int)
                    {
                        delay = (float)value;
                        if (delay == 0)
                            Step();
                    }
                    else if (value is IEnumerator)
                    {
                        stack.Push((IEnumerator)value);
                        Step();
                    }
                }
                else if (!ended)
                {
                    stack.Pop();
                    if (stack.Count <= 0)
                    {
                        Finished = true;
                        ended = true;
                        OnEnd?.Invoke();
                    }
                }
            }
        }

    }
}
