using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class StatTracker : MonoBehaviour
{
    public int AmmoCount;
    public float HealthPoints;
    public bool HasFirearm;

    public TMP_Text healthBar;
    public TMP_Text ammoBar;

    // Start is called before the first frame update
    void Start()
    {
        AmmoCount = 0;
        HealthPoints = 5;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.text = $"Health {HealthPoints} / 5";
        ammoBar.text = $"Ammo {AmmoCount} / 3";
    }

    public void UpdateHealthPoints( float healthEffect)
    {
        HealthPoints += healthEffect;
        HealthPoints = Mathf.Clamp(HealthPoints, 0, 5);
        if (HealthPoints == 0)
        {
            SceneManager.LoadScene("Mansion Beta");
        }
    }

    public void UpdateAmmoCount( int ammoEffect)
    {
        AmmoCount += ammoEffect;
        AmmoCount = Mathf.Clamp(AmmoCount, 0, 3);
    }
}
