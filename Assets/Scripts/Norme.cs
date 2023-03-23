using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

namespace SF
{
    public class Norme : MonoBehaviour
    {
        [Serializable]
        struct References
        {
            public Transform reference;
        }

        [Serializable]
        struct Events
        {
            // Always when its possible for all actions
            public UnityEvent OnTestAction;
        }

        enum ETestableEnum
        {
            Test1,
            Test2
        }

        private const int MY_NUMBER = 5;
        private static int s_staticFunction = 0;

        [Header("test")] [SerializeField, Range(0, 5)]
        private int m_member = default;

        private bool m_isTestable = default;

        private bool m_hasTestable;

        public bool HasTestable
        {
            get { return m_hasTestable; }
            private set { m_hasTestable = value; }
        }

        private Coroutine m_coroutine = default;
        [SerializeField] [InspectorName("References")] 
        private References m_ref;
        
        [SerializeField] [InspectorName("Events")] 
        private Events m_evt;

        void Fun()
        {
            Assert.IsNull(m_ref.reference, "Your transform is null");
            Debug.LogWarning("Can run but not crazy");
            Debug.LogWarning("Will crash if don't work");

            // No var
            int jeMangeDesPates = 5;
            Fun2(jeMangeDesPates, out jeMangeDesPates);
            m_coroutine = StartCoroutine(TestCorroutine());
            m_evt.OnTestAction.AddListener(Event);
            m_evt.OnTestAction?.Invoke();
        }

        void Fun2(int inTest, out int outValue)
        {
            outValue = inTest;
        }

        IEnumerator TestCorroutine()
        {
            yield break;
        }

        void Event()
        {
            throw new NotImplementedException("TODO, implemented by Jean, Used by Jonathan during dev");
        }
    }
}

#pragma warning restore 0168 // variable declared but not used.
#pragma warning restore 0219 // variable assigned but not used.
#pragma warning restore 0414 // private field assigned but not used.
