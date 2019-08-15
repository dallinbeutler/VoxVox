namespace MainState

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives
open Aardvark.UI

type Primitive =
    | Box
    | Sphere


[<DomainType>]
type State =
    {
        currentModel    : Primitive
        cameraState     : CameraControllerState
        points          : (C4b * V3f) list
        SelectedColor   : ColorInput
    }
type Msg =
    | ToggleModel
    | CameraMessage of FreeFlyController.Message
    | Click of V3d
    | SetColor of ColorPicker.Action
    | Ignore


module MainState =
    let pGridMatch color (pos :V3d )  = 
        let round v = round (float v )
        
        color,
        V3f(round pos.X,
            round pos.Y, 
            round pos.Z )

    let update (m : State) (msg : Msg) =
        match msg with
        | ToggleModel -> 
            match m.currentModel with
                | Box -> { m with currentModel = Sphere }
                | Sphere -> { m with currentModel = Box }

        | CameraMessage msg ->
            { m with cameraState = FreeFlyController.update m.cameraState msg }
        | Click pos ->
            { m with points = (pGridMatch m.SelectedColor.c pos ) :: m.points }
        | Ignore -> m
        |SetColor ca -> match ca with |ColorPicker.Action.SetColor co -> {m with SelectedColor = co}
        
    let initial = { 
      currentModel = Box
      cameraState = FreeFlyController.initial
      points = [(C4b.Cyan, V3f.OOO);(C4b.Blue, V3f.OIO)]
      SelectedColor = {c =C4b.DarkMagenta}
      }
    
