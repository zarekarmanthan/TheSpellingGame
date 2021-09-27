using UnityEngine;
using System;

public class AddFriend : MonoBehaviour
{
    [SerializeField]
    private string displayName;

    public static Action<string> OnAddFriend = delegate { };

    public void SetAddFriendName(string name)
    {
        displayName = name;
    }

    public void AddNewFriend()
    {
        if (string.IsNullOrEmpty(displayName))
            return;

        OnAddFriend?.Invoke(displayName);

    }
}
