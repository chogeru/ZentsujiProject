using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.UIPacks.NeumorphOne
{
    public class PopupExample : MonoBehaviour
    {
        public string PopupName = "Simple Popup";
        public bool AddToQueue = false;
        [Space(20)]
        public string Title = "Le Popup";
        public string Message = "This is a message";
        public string ButtonLabel = "Ole!";
        public UnityEvent ButtonClickEvent = new UnityEvent();

        private void Awake()
        {
            ButtonClickEvent.AddListener(() => Debug.Log($"Button '{ButtonLabel}' was clicked!"));
        }

        public void ShowPopup()
        {
            var popup = UIPopup.Get(PopupName);
            if(popup == null)
            {
                Debug.Log($"Popup '{PopupName}' does not exist in the database. Check spelling or if it was added via a {nameof(UIPopupLink)}");
                return;
            }

            popup
                .SetTexts(Title, Message, ButtonLabel)
                .SetEvents(ButtonClickEvent);

            if(AddToQueue)
            {
                popup.ShowFromQueue();
                return;
            }

            popup.Show();
        }
    }
}
