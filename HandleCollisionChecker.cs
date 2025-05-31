using UnityEngine;

public class HandleCollisionChecker : MonoBehaviour
{
    public MissionManager missionManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cürük"))
        {
            Debug.Log("Ayna sapý çürüðe temas etti!");

            // Görev tamamlanýyor
            if (missionManager.GetCurrentTask() == 3) // 4. görev
            {
                missionManager.CompleteCurrentTask();
            }
        }
    }
}