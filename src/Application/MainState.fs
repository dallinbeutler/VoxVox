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
        HoveredVox      : V3d option
        Plane           : Box3d
    }
type Msg =
    | ToggleModel
    | CameraMessage of FreeFlyController.Message
    | Click of V3d //SceneHit
    | Hover of V3d 
    | HoverExit
    | SetColor of ColorPicker.Action
    | Ignore




module MainState =
    let pGridMatch color (pos :V3d )  = 
        let round v = round (float v )
        
        color,
        V3f(round pos.X,
            round pos.Y, 
            round pos.Z )
    
    //let intersect (m:State) (r : Ray3d) =          
        //let mutable hit = RayHit3d.MaxRange
        //let result =
        //    m.points
        //    |> Array.tryFind(fun x -> 
                
        //        r.
        //        r.hi(x, 0.0, 100.0, &hit))

        //    result |> Option.map(fun x -> (x,hit))

    let update (m : State) (msg : Msg) =
        let adjPickPos pos = 
            pos - m.cameraState.view.Forward
        match msg with
        | ToggleModel -> 
            match m.currentModel with
                | Box -> { m with currentModel = Sphere }
                | Sphere -> { m with currentModel = Box }

        | CameraMessage msg ->
            { m with cameraState = FreeFlyController.update m.cameraState msg }
        | Click pos ->
            { m with points = (pGridMatch m.SelectedColor.c (pos + V3d(0.0,0.0,0.2)) ) :: m.points }
        //| Click hit ->
        //  let r = hit.globalRay.Ray.Ray
        //  match r |> intersect model with
        //  | Some (_,h) -> 
        //    let hitpoint = r.GetPointOnRay(h.T)
        //    { model with hitPoint = Some hitpoint }
        //  | None -> 
        //    Log.error "no hit"
        //    { model with hitPoint = None }
        //| KeyDown k ->
        | Ignore -> m
        |SetColor ca -> match ca with |ColorPicker.Action.SetColor co -> {m with SelectedColor = co}
        |Hover point -> {m with HoveredVox = Some point }
        |HoverExit -> {m with HoveredVox = None }
        
    let initial = { 
      Plane = Box3d(-V3d(1.0,1.0,0.0) * 10.0, V3d(1.0,1.0,0.01) *10.0)
      currentModel = Box
      cameraState = FreeFlyController.initial
      points = [(C4b.Cyan, V3f.OOO);(C4b.Blue, V3f.OIO)]
      SelectedColor = {c =C4b.DarkMagenta}
      HoveredVox = None
      }
    
