module Year2019Day14

open AdventOfCode.FSharp.Common

type Ingredient = { Name : string; Amount : int64 }
let asIngredient = splitBy " " (fun t -> { Name = t.[1]; Amount = int64 t.[0] })

type Recipe = { AmountProduced : int64; Ingredients : Ingredient array }
let asRecipe line =
    let lhs, rhs = splitBy " => " (fun t -> t.[0], t.[1]) line
    let ingredients = splitBy ", " (Array.map asIngredient) lhs
    let producedIngredient = asIngredient rhs
    producedIngredient.Name, { AmountProduced = producedIngredient.Amount; Ingredients = ingredients }

// If value is set to 0, remove it instead of adding it
let setItem k v m = if v = 0L then Map.remove k m else Map.add k v m
let findOrZero k m = Map.tryFind k m |> Option.defaultValue 0L

type RecipeState = 
    { Recipes : Map<string, Recipe>
      ItemsToMake : Map<string, int64>
      LeftoverItems : Map<string, int64> }

    static member create recipes = { Recipes = recipes; ItemsToMake = Map.empty; LeftoverItems = Map.empty }
    static member setItemsToMake item amount s = { s with ItemsToMake = setItem item amount s.ItemsToMake }
    static member setLeftovers item amount s = { s with LeftoverItems = setItem item amount s.LeftoverItems }

    static member addNewItemsToBeMade item amount s =
        let leftovers = findOrZero item s.LeftoverItems
        let toBeMade = amount + (findOrZero item s.ItemsToMake)

        s
        |> RecipeState.setLeftovers   item (if leftovers >= toBeMade then leftovers - toBeMade else 0L)
        |> RecipeState.setItemsToMake item (if leftovers <= toBeMade then toBeMade - leftovers else 0L)

let ceilDivision a b = (a / b) + (if a % b = 0L then 0L else 1L)

let rec reduceItemsToOre recipeState = 
    match Map.tryPick (fun k v -> if k <> "ORE" then Some (k, v) else None) recipeState.ItemsToMake with
    | Some (item, amountToMake) ->
        let recipe = recipeState.Recipes.[item]
        let recipesToMake = ceilDivision amountToMake recipe.AmountProduced
        
        recipe.Ingredients
        |> Array.fold (fun state ing -> RecipeState.addNewItemsToBeMade ing.Name (recipesToMake * ing.Amount) state) recipeState
        |> RecipeState.setItemsToMake item 0L
        |> RecipeState.setLeftovers item (recipe.AmountProduced * recipesToMake - amountToMake)
        |> reduceItemsToOre
    | None -> recipeState.ItemsToMake.["ORE"]

let getTotalOre fuel = 
    RecipeState.create
    >> RecipeState.setItemsToMake "FUEL" fuel
    >> reduceItemsToOre

let solvePart1 = getTotalOre 1L
    
let solvePart2 recipes = 
    let target = 1000000000000L
    let rec tryFuel prevFuel oreUsed =
        match oreUsed with
        | Some ore -> 
            let nextFuel = max (prevFuel + 1L) (prevFuel * target / ore - 1L)
            let nextOreUsed = getTotalOre nextFuel recipes
            if nextOreUsed < target then 
                tryFuel nextFuel (Some nextOreUsed)
            else prevFuel
        | None -> tryFuel prevFuel (Some (getTotalOre prevFuel recipes))
    tryFuel 1L None
    

let solver = { parse = parseEachLine asRecipe >> Map.ofSeq; part1 = solvePart1; part2 = solvePart2 }