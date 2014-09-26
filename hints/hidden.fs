﻿module hints.hidden

open System
open System.Text

open console

open core.setCell
open core.sudoku
open hints

let findHidden (cellHouseCells : Cell -> Set<Cell>) (candidateLookup : Cell -> Set<Candidate>) (primaryHouseCells : Set<Cell>) (candidateSubset : Set<Candidate>) 
    (count : int) (primaryHouse : House) = 
    let pairs = 
        List.map (fun cell -> 
            let candidates = candidateLookup cell
            
            let pointer = 
                { CandidateReduction.cell = cell
                  candidates = Set.intersect candidates candidateSubset }
            
            let crs = 
                if Set.count pointer.candidates > 0 then Set.difference candidates candidateSubset
                else set []
            
            let candidateReduction = 
                { CandidateReduction.cell = cell
                  candidates = crs }
            
            (pointer, candidateReduction)) (Set.toList primaryHouseCells)
    
    let pointers, candidateReductions = List.unzip pairs

    let nonEmptyPointers = List.filter (fun cr -> Set.count cr.candidates > 0) pointers

    let nonEmptyCandidateReductions = List.filter (fun cr -> Set.count cr.candidates > 0) candidateReductions

    if List.length nonEmptyPointers = count && List.length nonEmptyCandidateReductions > 0 then 

        let setCellValue = 
            if count = 1 then 
                let h = List.head nonEmptyPointers
                let cell = h.cell
                let candidate = first candidateSubset

                let setCellValue = makeSetCellValue cell candidate cellHouseCells candidateLookup

                Some setCellValue
            else None

        Some { HintDescription.primaryHouses = set [ primaryHouse ]
               primaryHouseCells = primaryHouseCells
               secondaryHouses = set []
               secondaryHouseCells = set []
               candidateReductions = Set.ofList nonEmptyCandidateReductions
               setCellValue = setCellValue
               pointers = Set.ofList nonEmptyPointers }
    else None

let hiddenNPerHouse (cellHouseCells : Cell -> Set<Cell>) (candidateLookup : Cell -> Set<Candidate>) (houseCells : House -> Set<Cell>) (count : int) 
    (house : House) = 
    let cells = houseCells house

    let houseCandidates = Set.map candidateLookup cells |> Set.unionMany

    let candidateSubsets = setSubsets (Set.toList houseCandidates) count
    let hs = 
        List.map 
            (fun candidateSubset -> 
            findHidden cellHouseCells candidateLookup cells (Set.ofList candidateSubset) count house) candidateSubsets
    List.choose id hs
