using UnityEngine;

public class BubbleEmitter : MonoBehaviour
{
    public GameObject BubblePrefab;

    public async void SpawnBubble()
    {
        Spawn();
        await Awaitable.WaitForSecondsAsync(5f);
        SpawnBubble();
    }

    public async void SpawnInitialBubbles()
    {
        for (int i = 0; i < 100; i++)
        {
            Spawn();
            await Awaitable.WaitForSecondsAsync(0.003f);
        }

        SpawnBubble();
    }

    private void Spawn()
    {
        var randomPosition = new Vector3(Random.Range(-.3f, .3f), 0f, Random.Range(-.3f, .3f));
        Instantiate(BubblePrefab, transform.position + randomPosition, Quaternion.identity);
    }
}
