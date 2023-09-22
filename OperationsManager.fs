module Server.OperationsManager
open System
open Microsoft.FSharp.Collections

let operationSet = Set.empty.Add("mul").Add("add").Add("sub")



// first check input validity
// if valid we parse the input
// we send the answer back to the client
// if its not valid we send an error code

let rec errorScenario (commandArgs : string[]) : string[] =
    let mutable newArgs : string array = Array.zeroCreate 3
    for i in 0 .. 2 do
        if i = 2 then
            if commandArgs[i].Length < 3 then
                newArgs[i] <- commandArgs[i].Substring(0, commandArgs[2].Length - 1)
            else
                newArgs[i] <- commandArgs[i]
        else
            newArgs[i] <- commandArgs[i]
    newArgs
let fetchArgs (command : string) : string[] =
    let separators = [|' '|]
    let commandArgs = command.Trim().Split (separators, StringSplitOptions.RemoveEmptyEntries)
    for i in 0 .. commandArgs.Length - 1 do 
        printfn $"%s{commandArgs[i]}"
    commandArgs
    
let inputValidator (input : string) : bool =
    let commandArgs = fetchArgs input
    if operationSet.Contains(commandArgs[0].ToLower()) then
        printfn $"Operation entered was %s{commandArgs[0]}"
        // check if the number of arguments after the operation is <= 4
        not (commandArgs.Length > 5 || commandArgs.Length < 3)
    else
        false

let addNumbers (nums : string[]) : int =
    let mutable sum = 0
    for i in 0 .. nums.Length - 1 do
        sum <- int(nums[i].Trim()) + sum
    sum
    
let subtractNumbers (nums : string[]) : int =
    let diff = int(nums[0].Trim()) - int(nums[1].Trim())
    diff
 
let multiplyNumbers (nums : string[]) : int =
    let mutable res = 1
    for i in 0..nums.Length - 1 do
        res <- res * int(nums[i].Trim())
    res
    
let operationsManager (input : string) : int =
    let isValid = inputValidator input
    if isValid then
        let mutable result = -1
        let mutable commandArgs = fetchArgs input
        if commandArgs[0].Equals("add") then
            if commandArgs.Length = 3 then
                // its because of the bell character apparently?
                // its represented by char(7)
                commandArgs <- errorScenario commandArgs
                result <- addNumbers commandArgs[1..]
            else
                result <- addNumbers commandArgs[1..] 
        elif commandArgs[0].Equals("sub") then
            result <- subtractNumbers commandArgs[1..]
        elif commandArgs[0].Equals("mul") then
            result <- multiplyNumbers commandArgs[1..]
        result
    else
        -1