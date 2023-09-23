module Server.OperationsManager
open System
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections

[<Literal>]
let Addition = "add"

[<Literal>]
let Subtraction = "subtract"

[<Literal>]
let Multiplication = "multiply"

[<Literal>]
let EXIT_COMMAND = "bye"

[<Literal>]
let TERMINATE = "terminate"

[<Literal>]
let EXIT_CODE = "-5"

[<Literal>]
let TERMINATION_CODE = "-6"

let operationSet = Set.empty.Add(Addition).Add(Subtraction).Add(Multiplication).Add(TERMINATE).Add(EXIT_COMMAND)

(* first check input validity if valid we parse the input
 we send the answer back to the client if its not valid we
 send an error code *)

let removeNonPrintableASCIIChars (commandArgs : string[]) : string[] =
    let mutable newArgs = Array.zeroCreate commandArgs.Length
    let pattern = "[^ -~]+"
    let regex = Regex(pattern)
    for i in 0..commandArgs.Length - 1 do
        newArgs[i] <- regex.Replace(commandArgs[i], "")
    newArgs

let isValidNumericInput (commandArgs : string[]) : bool =
    let pattern = "^[0-9]+$"
    let regex = Regex(pattern)
    let mutable isValidString = true
    for i in 0..commandArgs.Length - 1 do
        isValidString <- isValidString && regex.IsMatch(commandArgs[i])
    isValidString
       
        
        
let fetchArgs (command : string) : string[] =
    let separators = [|' '|]
    let commandArgs = command.Trim().Split (separators, StringSplitOptions.RemoveEmptyEntries)
    commandArgs
    
let inputValidator (input : string) : bool =
    let mutable commandArgs = fetchArgs input
    commandArgs <- removeNonPrintableASCIIChars commandArgs
    if operationSet.Contains(commandArgs[0].ToLower()) then
        if commandArgs[0].Equals(EXIT_COMMAND) then
            commandArgs.Length = 1
        else
            
        // check if the number of arguments after the operation is <= 4
        let mutable isValid = not (commandArgs.Length > 5 || commandArgs.Length < 3)
        isValid <- isValid && isValidNumericInput commandArgs[1..]
        isValid
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
        
        if commandArgs[0].Equals(Addition) then
            // Non-printable ASCII characters were appearing
            // in the input received from client, hence we
            // are replacing those with ""
            commandArgs <- removeNonPrintableASCIIChars commandArgs
            result <- addNumbers commandArgs[1..]
        elif commandArgs[0].Equals(Subtraction) then
            commandArgs <- removeNonPrintableASCIIChars commandArgs
            result <- subtractNumbers commandArgs[1..]
        elif commandArgs[0].Equals(Multiplication) then
            commandArgs <- removeNonPrintableASCIIChars commandArgs
            result <- multiplyNumbers commandArgs[1..]
        elif commandArgs[0].Contains(EXIT_COMMAND) then
            // only the client disconnects and shuts down in this case
            result <- int(EXIT_CODE)
        elif commandArgs[0].Contains(TERMINATE) then
            // we need to close the server socket and any dangling threads,
            // while client disconnects and shuts down as well
            result <- int(TERMINATION_CODE)
        result
    else
        let mutable commandArgs : string[] = fetchArgs input
        commandArgs <- removeNonPrintableASCIIChars commandArgs
        // check reason for invalidity
        if(operationSet.Contains(commandArgs[0].ToLower())) then
            // the operation is correct
            if commandArgs.Length > 2 then
                // the number of inputs is more than 2
                if commandArgs.Length <= 5 then
                    // the number of inputs is less than or equal to 4
                    // so the error has to be that the input contains one or more non-number(s)
                    -4
                else
                    // the number of inputs provided is more than 4
                    -3
            else
                // the number of inputs provided is less than 2
                -2
        else
            // the operation is not valid
            -1
        