using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    private PlayerShoot _playershoot;


    private void Start()
    {
        TryFindPlayerShoot();
    }

    private void Update()
    {

        if(_playershoot == null){
            TryFindPlayerShoot();
        }
        else{
            SetHealth(_playershoot.health);
        }
    }

    private void TryFindPlayerShoot(){
        _playershoot = LocalPlayerManager.Instance?.playerShoot; //returns null if not found

        if (_playershoot != null)
        {
            Debug.Log("✅ PlayerShoot found and assigned to HealthBar!");
            SetMaxHealth(_playershoot.maxHealth);
        }

    }

    public void SetMaxHealth(int health){
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health){
        slider.value = health;
    }
}
