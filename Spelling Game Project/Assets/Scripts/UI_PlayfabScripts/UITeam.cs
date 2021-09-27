using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    [SerializeField] private int _teamSize;
    [SerializeField] private int _maxTeamSize;
    [SerializeField] private PhotonTeam _team;
    [SerializeField] private TMP_Text _teamNameText;
    [SerializeField] private Transform _playerSelectionContainer;
    [SerializeField] private UIPlayerSelection _playerSelectionPrefab;
    [SerializeField] private Dictionary<Player, UIPlayerSelection> _playerSelections;
    [SerializeField] private Image _animalImage;
    [SerializeField] private GameObject _previousButton;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private int _currentSelection;
    [SerializeField] private Player _owner;

    public static Action<PhotonTeam> OnSwitchToTeam = delegate { };


    private void Awake()
    {
        UIDisplayTeam.OnAddPlayerToTeam += HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam += HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
    }

    private void OnDestroy()
    {
        UIDisplayTeam.OnAddPlayerToTeam -= HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam -= HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
    }

    public void InitializeAvatar()
    {
        _currentSelection = 0;

         _previousButton.SetActive(true);
        _nextButton.SetActive(true);

        UpdateCharacterModel(_currentSelection);
    }

    public void Initialize(PhotonTeam team, int teamSize)
    {
        _team = team;
        _maxTeamSize = teamSize;
        Debug.Log($"{_team.Name} is added with the size {_maxTeamSize}");
        _playerSelections = new Dictionary<Player, UIPlayerSelection>();
        UpdateTeamUI();

        Player[] teamMembers;
        if (PhotonTeamsManager.Instance.TryGetTeamMembers(_team.Code, out teamMembers))
        {
            foreach (Player player in teamMembers)
            {
                AddPlayerToTeam(player);
            }

            InitializeAvatar();
        }
       /* if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == team.Code)
        {

        }*/
    }

    private void UpdateCharacterModel(int selection)
    {
        _animalImage.sprite = _sprites[selection];
    }



    public void PreviousSelection()
    {
        _currentSelection--;
        if (_currentSelection < 0)
        {
            _currentSelection = _sprites.Length - 1;
        }
        _animalImage.sprite = _sprites[_currentSelection];
    }

    public void NextSelection()
    {
        _currentSelection++;
        if (_currentSelection > _sprites.Length - 1)
        {
            _currentSelection = 0;
        }
        _animalImage.sprite = _sprites[_currentSelection];

    }



    public void HandleAddPlayerToTeam(Player player, PhotonTeam team)
    {
        if (_team.Code == team.Code)
        {
            Debug.Log($"Updating {_team.Name} UI to add {player.NickName}");
            AddPlayerToTeam(player);
        }
    }

    public void HandleRemovePlayerFromTeam(Player player)
    {
        RemovePlayerFromTeam(player);
    }

    private void HandleLeaveRoom()
    {
        Destroy(gameObject);
    }

    private void UpdateTeamUI()
    {
        _teamNameText.SetText($"{_team.Name} \n {_playerSelections.Count} / {_maxTeamSize}");
    }

    private void AddPlayerToTeam(Player player)
    {
        UIPlayerSelection uiPlayerSelection = Instantiate(_playerSelectionPrefab, _playerSelectionContainer);
        uiPlayerSelection.Initialize(player);
        _playerSelections.Add(player, uiPlayerSelection);
        UpdateTeamUI();
    }

    private void RemovePlayerFromTeam(Player player)
    {
        if (_playerSelections.ContainsKey(player))
        {
            Debug.Log($"Updating {_team.Name} UI to remove {player.NickName}");
            Destroy(_playerSelections[player].gameObject);
            _playerSelections.Remove(player);
            UpdateTeamUI();
        }
    }

    public void SwitchToTeam()
    {
        Debug.Log($"Trying to switch to team {_team.Name}");
        if (_teamSize >= _maxTeamSize) return;

        Debug.Log($"Switching to team {_team.Name}");
        OnSwitchToTeam?.Invoke(_team);
    }
}
