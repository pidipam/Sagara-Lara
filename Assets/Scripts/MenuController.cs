using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenu;      // Panel menu utama
    public GameObject settingsMenu;  // Panel menu settings

    // Fungsi untuk membuka menu settings
    public void OpenSettings()
    {
        mainMenu.SetActive(false);      // Sembunyikan menu utama
        settingsMenu.SetActive(true);   // Tampilkan menu settings
    }

    // Fungsi untuk menutup menu settings dan kembali ke menu utama
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);  // Sembunyikan menu settings
        mainMenu.SetActive(true);       // Tampilkan menu utama
    }
}
