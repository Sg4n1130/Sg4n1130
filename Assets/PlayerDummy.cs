using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    public void TakeDamage(float amount)
    {
        Debug.Log($"플레이어 피격! 데미지: {amount}");
    }
}