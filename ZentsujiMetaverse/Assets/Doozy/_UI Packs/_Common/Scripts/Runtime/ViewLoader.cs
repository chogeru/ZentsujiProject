using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEngine;

namespace Doozy.UIPacks.NeumorphOne
{
    public class ViewLoader : MonoBehaviour
    {
        public List<UIView> Views;
        public UIView CurrentView;

        private SignalReceiver receiver { get; set; }

        private void Awake()
        {
            receiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
        }

        private void OnEnable()
        {
            UIButton.stream.ConnectReceiver(receiver);
            Views.RemoveNulls();
        }

        private void OnDisable()
        {
            UIButton.stream.DisconnectReceiver(receiver);
        }

        private void OnSignal(Signal signal)
        {
            if (!signal.hasValue) return;
            if (signal.valueType != typeof(UIButtonSignalData)) return;
            UIButtonSignalData data = signal.GetValueUnsafe<UIButtonSignalData>();
            switch (data.buttonName)
            {
                case "Previous" : 
                    LoadPrevious();
                    break;
                case "Next" : 
                    LoadNext();
                    break;
            }
        }

        public void LoadPrevious()
        {
            if (Views == null || Views.Count == 0) return;
            if (CurrentView != null)
            {
                CurrentView.Hide();
                int index = Views.IndexOf(CurrentView);
                index--;
                if (index < 0) index = Views.Count - 1;
                CurrentView = Views[index];
            }
            else
            {
                CurrentView = Views[Views.Count - 1];
            }

            CurrentView.Show();
        }

        public void LoadNext()
        {
            if (Views == null || Views.Count == 0) return;
            if(CurrentView != null)
            {
                CurrentView.Hide();
                int index = Views.IndexOf(CurrentView);
                index++;
                if (index >= Views.Count) index = 0;
                CurrentView = Views[index];
            }
            else
            {
                CurrentView = Views[0];
            }
            
            CurrentView.Show();
        }
    }
}
