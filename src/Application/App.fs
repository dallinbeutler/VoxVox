namespace Application

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI
open Aardvark.UI.Primitives
open Aardvark.Base.Rendering
open Aardvark.SceneGraph
//open Application.Model


module App =
    
    let view (m : MainState.Mutable.MState) =

        let frustum = 
            Frustum.perspective 60.0 0.1 100.0 1.0 
                |> Mod.constant
        let poo (sh:SceneHit) = 
            sh.globalPosition - sh.globalRay.Ray.InvDir
        let sg =            
            let plane = 
                Sg.box (Mod.constant C4b.White) (Mod.constant (Box3d(-V3d(1.0,1.0,0.01) * 10.0, V3d(1.0,1.0,0.01) *10.0) ))
                //Sg.li
            let cubes =                 
                aset{
                    let! curShape = m.currentModel
                    let shape = 
                        match curShape with
                        |MainState.Box -> fun c -> Sg.box (Mod.constant c) (Mod.constant (Box3d(-V3d(0.5,0.5,0.5), V3d(0.5,0.5,0.5))))
                        |MainState.Sphere -> fun c-> Sg.sphere 5 (Mod.constant c) (Mod.constant 0.5)
                    let! points = m.points
                    for p in points do
                    let loc=  snd p
                    yield 
                        shape (fst p)
                        |> Sg.translate (float loc.X) (float loc.Y) (float loc.Z)  
                        //|> Sg.requirePicking
                        //|> Sg.noEvents
                        //|> Sg.withEvents[ Sg.onClick (fun _ -> MainState.Ignore) 
                        //                 ]
                    }
                    //|> Aardvark.UI.Sg.Set
                    |> Sg.set

            Sg.group'[
                plane
                cubes
            ]
            
            |> Sg.requirePicking            
            |> Sg.noEvents
            
            //|> Sg.withEvents[Sg.onMouseDown(fun butt pos  -> if butt = Aardvark.Application.MouseButtons.Left then  MainState.Click pos else MainState.Ignore)]
            |> Sg.withEvents[Sg.onClick (fun pos -> MainState.Click pos) ]
            :> ISg 
            |> Sg.shader {
                do! DefaultSurfaces.trafo
                do! DefaultSurfaces.simpleLighting
            } |> Sg.noEvents
        let dependencies = 
            Html.semui @ [        
                { name = "spectrum.js";  url = "spectrum.js";  kind = Script     }
                { name = "spectrum.css";  url = "spectrum.css";  kind = Stylesheet     }
            ] 
        let att =
            [
                style "position: fixed; left: 0; top: 0; width: 100%; height: 100%"
            ]
        require dependencies (
                body [] [
                    FreeFlyController.controlledControl m.cameraState MainState.CameraMessage frustum (AttributeMap.ofList att) sg

                    div [style "position: fixed; left: 20px; top: 20px width: 200;"] [
                        button [onClick (fun _ -> MainState.ToggleModel)] [text "Toggle Model"]
                        ColorPicker.view m.SelectedColor |> UI.map  MainState.SetColor
                        //button [style sprintf "background: %s" (Html.ofC4b c)][fill]
                    ]

                ])

    let app =
        {
            initial = MainState.MainState.initial
            update = MainState.MainState.update
            view = view
            threads = MainState.Mutable.State.Lens.cameraState.Get >> FreeFlyController.threads >> ThreadPool.map MainState.CameraMessage
            unpersist = Unpersist.instance
        }