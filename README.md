# NutritionizerAPI

This is very much a WIP.  I will continue developing it as I learn more about ASP.NET APIs.  Currently, it connects to a PostgreSQL DB using Entity Framework and allows you to GET a recipe by its ID.

Table schemas are as follows:
```
                                        Table "public.recipes"
  Column   |          Type          | Collation | Nullable |                  Default
-----------+------------------------+-----------+----------+-------------------------------------------
 recipe_id | integer                |           | not null | nextval('recipes_recipeid_seq'::regclass)
 web_id    | integer                |           | not null |
 rating    | numeric(3,2)           |           | not null |
 url       | character varying(256) |           | not null |
 website   | character varying(32)  |           | not null |
Indexes:
    "recipes_pkey" PRIMARY KEY, btree (recipe_id)
```
```
                                            Table "public.ingredients"
     Column     |         Type          | Collation | Nullable |                      Default
----------------+-----------------------+-----------+----------+---------------------------------------------------
 ingredient_id  | integer               |           | not null | nextval('ingredients_ingredientid_seq'::regclass)
 recipe_id      | integer               |           | not null |
 quantity       | numeric(6,4)          |           | not null |
 description    | character varying(64) |           | not null |
 recipe_section | character varying(32) |           | not null |
 unit           | character(16)         |           | not null |
Indexes:
    "ingredients_pkey" PRIMARY KEY, btree (ingredient_id)
```

The JSON string of a GET request returns something like this:
```
    {
      "recipeId": 0,
      "webId": 12345,
      "rating": 5.5,
      "website": "a.recipe.com",
      "url": "a.recipe.com/carrotanas",
      "ingredients": [
        {
          "ingredientId": 0,
          "recipeId": 0,
          "quantity": 1,
          "description": "Carrot, chopped",
          "recipeSection": "Base:",
          "unit": "Cup"
        },
        {
          "ingredientId": 1,
          "recipeId": 0,
          "quantity": 1,
          "description": "Banana, sliced",
          "recipeSection": "Base:",
          "unit": "Tbsp"
        }
      ]
    }
```

I am planning on adding full CRUD, and once that's done, I will write a browser extension that parses a website with a recipe and sends the data to the API server.
