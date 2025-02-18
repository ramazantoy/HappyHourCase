

[System.Serializable]
public class PlayerSkills {
    public bool arrowMultiplication;    // Normalde 1 ok yerine 2 ok atılır.
    public bool bounceDamage;           // Ok, enemy’ye çarptıktan sonra yakın additional enemy’e sekebilir.
    public bool burnDamage;             // Ok, enemy’ye yanık hasarı uygular.
    public bool attackSpeedIncrease;    // Ok atma hızını 2 kat artırır.
    public bool rageMode;               // Tüm efektler iki kat (veya ilgili durumda 4 kat) güçlenir.
}