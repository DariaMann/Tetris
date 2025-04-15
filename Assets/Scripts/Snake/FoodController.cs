using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    [SerializeField] private Collider2D gridArea;
    [SerializeField] private GameObject foodPrefab;
    
    public List<Food> Foods { get; set; } = new List<Food>();
    
    public Collider2D GridArea
    {
        get => gridArea;
        set => gridArea = value;
    }
    
    public void CreateNewFoods()
    {
        int range = 1;
        if (GameHelper.SnakeSettings.ManyFood)
        {
             int min = Foods.Count > 0 ? 0 : 1;
             range = Random.Range(min, 3);
             Debug.Log("Range: " + range);
        }
        
        while (range > 0)
        {
            CreateFood();
            range -= 1;
        }
    }

    public void CreateFood(SaveFood saveFood = null)
    {
        GameObject food = Instantiate(foodPrefab);
        Food newFood = food.GetComponent<Food>();
        if (saveFood == null)
        {
            newFood.RandomizePosition();
        }
        else
        {
            newFood.LoadPosition(new Vector2(saveFood.X, saveFood.Y));
        }
       
        Foods.Add(newFood);
    }
    
    public void LoadedFood(List<SaveFood> saveFoods)
    {
        foreach (var saveFood in saveFoods)
        {
            CreateFood(saveFood);
        }
    }

    public void Reset()
    {
        foreach (var food in Foods)
        {
            Destroy(food.gameObject);
        }
        
        Foods.Clear();
    }
}