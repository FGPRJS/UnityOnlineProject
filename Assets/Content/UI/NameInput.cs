using TMPro;
using UnityEngine;

namespace Content.UI
{
    public class NameInput : MonoBehaviour
    {
        public TMP_InputField inputField;

        public string GetInputName()
        {
            return inputField.text;
        }
    }
}
