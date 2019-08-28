module Octree

open Aardvark.Base
open Aardvark
open Aardvark.Rendering
type Vox = uint16 //int represents material index. materials hold physics and rendering properties as welll

type VoxOct = 
    struct
        val Points:Node array
        member this.OOO = this.Points.[0]
        member this.OOI = this.Points.[1]
        member this.OIO = this.Points.[2]
        member this.OII = this.Points.[3]
        member this.IOO = this.Points.[4]
        member this.IOI = this.Points.[5]
        member this.IIO = this.Points.[6]
        member this.III = this.Points.[7]
        member this.At (X:bool) (Y:bool) (Z:bool) =
            this.Points.[(if X then 4 else 0) + (if Y then 2 else 0) + (if Z then 1 else 0)]
        member this.col (X:int)(Y:int)(Z:int)  =
            match (this.At ((X % 2) == 1) (Y % 2) (Z % 2)) with
            |
            
    end
and Node = 
|Single of Vox
|Sub of VoxOct

//let checkColl (point:V3i) (scale: int) =
//   if point.X > 1 then 