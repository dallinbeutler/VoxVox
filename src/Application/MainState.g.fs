namespace MainState

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open MainState

[<AutoOpen>]
module Mutable =

    
    
    type MState(__initial : MainState.State) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<MainState.State> = Aardvark.Base.Incremental.EqModRef<MainState.State>(__initial) :> Aardvark.Base.Incremental.IModRef<MainState.State>
        let _currentModel = ResetMod.Create(__initial.currentModel)
        let _cameraState = Aardvark.UI.Primitives.Mutable.MCameraControllerState.Create(__initial.cameraState)
        let _points = ResetMod.Create(__initial.points)
        let _SelectedColor = Aardvark.UI.Mutable.MColorInput.Create(__initial.SelectedColor)
        
        member x.currentModel = _currentModel :> IMod<_>
        member x.cameraState = _cameraState
        member x.points = _points :> IMod<_>
        member x.SelectedColor = _SelectedColor
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : MainState.State) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_currentModel,v.currentModel)
                Aardvark.UI.Primitives.Mutable.MCameraControllerState.Update(_cameraState, v.cameraState)
                ResetMod.Update(_points,v.points)
                Aardvark.UI.Mutable.MColorInput.Update(_SelectedColor, v.SelectedColor)
                
        
        static member Create(__initial : MainState.State) : MState = MState(__initial)
        static member Update(m : MState, v : MainState.State) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<MainState.State> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module State =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let currentModel =
                { new Lens<MainState.State, MainState.Primitive>() with
                    override x.Get(r) = r.currentModel
                    override x.Set(r,v) = { r with currentModel = v }
                    override x.Update(r,f) = { r with currentModel = f r.currentModel }
                }
            let cameraState =
                { new Lens<MainState.State, Aardvark.UI.Primitives.CameraControllerState>() with
                    override x.Get(r) = r.cameraState
                    override x.Set(r,v) = { r with cameraState = v }
                    override x.Update(r,f) = { r with cameraState = f r.cameraState }
                }
            let points =
                { new Lens<MainState.State, Microsoft.FSharp.Collections.List<(Aardvark.Base.C4b * Aardvark.Base.V3f)>>() with
                    override x.Get(r) = r.points
                    override x.Set(r,v) = { r with points = v }
                    override x.Update(r,f) = { r with points = f r.points }
                }
            let SelectedColor =
                { new Lens<MainState.State, Aardvark.UI.ColorInput>() with
                    override x.Get(r) = r.SelectedColor
                    override x.Set(r,v) = { r with SelectedColor = v }
                    override x.Update(r,f) = { r with SelectedColor = f r.SelectedColor }
                }
