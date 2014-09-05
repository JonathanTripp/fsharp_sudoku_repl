﻿module hints.hiddenPair

open core.sudoku
open hints

val hiddenPairFind : Candidate list
     -> (Cell -> Set<Candidate>) -> (House -> Set<Cell>) -> House list -> HintDescription list