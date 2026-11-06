using System.Collections.Generic;
using UnityEngine;

public class ItemSwapper : MonoBehaviour
{
    [Header("Кости рук всех жителей (B-hand_R)")]
    public Transform[] hands;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomizeItems();
        }
    }

    void RandomizeItems()
    {
        if (hands == null || hands.Length == 0) return;

        int n = hands.Length;
        Transform[] currentItems = new Transform[n];
        Transform[] shuffledItems = new Transform[n];

        Dictionary<Transform, Vector3> localPositions = new Dictionary<Transform, Vector3>();
        Dictionary<Transform, Quaternion> localRotations = new Dictionary<Transform, Quaternion>();

        for (int i = 0; i < n; i++)
        {
            Transform item = GetItemInHand(hands[i]);
            currentItems[i] = item;
            shuffledItems[i] = item;

            if (item != null)
            {
                localPositions[item] = item.localPosition;
                localRotations[item] = item.localRotation;
            }
        }

        int maxAttempts = 200;
        int attempts = 0;
        bool isValid = false;

        while (!isValid && attempts < maxAttempts)
        {
            ShuffleArray(shuffledItems);
            isValid = true;

            for (int i = 0; i < n; i++)
            {
                Transform oldItem = currentItems[i];
                Transform newItem = shuffledItems[i];

                if (oldItem == null && newItem == null)
                {
                    isValid = false;
                    break;
                }

                if (oldItem != null && newItem != null)
                {
                    string oldTypeName = GetCleanItemName(oldItem.name);
                    string newTypeName = GetCleanItemName(newItem.name);

                    if (oldTypeName == newTypeName)
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Не удалось найти идеальную комбинацию! Возможно, уникальных предметов слишком мало для текущего распределения.");
        }

        for (int i = 0; i < n; i++)
        {
            Transform newItem = shuffledItems[i];

            if (newItem != null)
            {
                newItem.SetParent(hands[i]);
                newItem.localPosition = localPositions[newItem];
                newItem.localRotation = localRotations[newItem];
            }
        }
    }

    void ShuffleArray(Transform[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    Transform GetItemInHand(Transform hand) // Ищем предметы, избегая пальцев
    {
        foreach (Transform child in hand)
        {
            if (!child.name.StartsWith("B-"))
            {
                return child;
            }
        }
        return null;
    }

    string GetCleanItemName(string originalName) // Проверка на клонов
    {
        int bracketIndex = originalName.IndexOf("(");
        if (bracketIndex > 0)
        {
            return originalName.Substring(0, bracketIndex).Trim();
        }
        return originalName.Trim();
    }
}