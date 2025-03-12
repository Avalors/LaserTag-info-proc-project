using UnityEngine;
using TMPro;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private TextMeshPro healthText;
    private PlayerShoot _playerShoot;

    private void Start()
    {
        _playerShoot = GetComponent<PlayerShoot>();
    }

    private void Update()
    {
        healthText.text = _playerShoot.health.ToString();
    }
}
