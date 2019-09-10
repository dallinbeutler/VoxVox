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
    open Aardvark.Application
    
    let view (m : MainState.Mutable.MState) =

        let frustum = 
            Frustum.perspective 60.0 0.1 100.0 1.0 
                |> Mod.constant
        let poo (sh:SceneHit) = 
            sh.globalPosition - sh.globalRay.Ray.InvDir
        let sg =            
            let plane = 
                Sg.box (Mod.constant C4b.White) ((m.Plane)) 
                |> Sg.requirePicking
                |> Sg.noEvents
                |> Sg.withEvents [Sg.onClick (fun pos -> MainState.Click pos)]
                :> ISg
                |> Sg.shader {
                        do! DefaultSurfaces.trafo
                        do! DefaultSurfaces.simpleLighting
                        }
            //let cubes = 
            //    alist{
            //        for p in m.points do
            //            let loc=  snd p
                        
            //            shape (fst p)
            //            |> Sg.translate (float loc.X) (float loc.Y) (float loc.Z) 
            //            |> Sg.requirePicking
            //            |> Sg.fillMode (Mod.constant FillMode.Fill)
            //            |> Sg.cullMode (Mod.constant CullMode.None)
            //            |> Sg.noEvents
            //            |> Sg.withEvents [
            //                Sg.onClick (fun pos -> MainState.Click pos)
            //                Sg.onEnter (fun pos -> MainState.Hover (loc.ToV3d()))
            //                Sg.onLeave (fun _ -> MainState.HoverExit)
            //                ]
            //            :> ISg
            //        )
            //    }
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
                            |> Sg.requirePicking
                            |> Sg.fillMode (Mod.constant FillMode.Fill)
                            |> Sg.cullMode (Mod.constant CullMode.None)
                            |> Sg.noEvents
                            |> Sg.withEvents [
                                Sg.onClick (fun pos -> MainState.Click pos)
                                Sg.onEnter (fun pos -> MainState.Hover (loc.ToV3d()))
                                Sg.onLeave (fun _ -> MainState.HoverExit)
                                ]
                            :> ISg

                    
                    let! hovered = m.HoveredVox
                    match hovered with 
                    |None -> ()
                    |Some p -> yield shape (C4b.Red) 
                               |> Sg.translate (float p.X) (float p.Y) (float p.Z) 
                               |> Sg.fillMode (Mod.constant FillMode.Line)
                    
                    
                    }
                    |> Sg.set
                    |> Sg.shader {
                    do! DefaultSurfaces.trafo
                    do! DefaultSurfaces.simpleLighting
                    }
            Sg.group'[
                plane
                cubes
            ]
            |> Sg.noEvents

            //|> Sg.requirePicking            
            
            //|> Sg.withEvents[Sg.onMouseDown(fun butt pos  -> if butt = Aardvark.Application.MouseButtons.Left then  MainState.Click pos else MainState.Ignore)]
            //|> Sg.withEvents[
            //                Sg.onClick (fun pos -> MainState.Click pos) 
            //                //SceneEventKind.Click, (
            //                // fun sceneHit ->
            //                //     true, Seq.ofList[MainState.Msg.Click (sceneHit)]
            //                //     )
            //                ]
            //:> ISg 
            //|> Sg.noEvents
            
            
            
        let dependencies = 
            Html.semui @ [        
                { name = "spectrum.js";  url = "spectrum.js";  kind = Script     }
                { name = "spectrum.css";  url = "spectrum.css";  kind = Stylesheet     }
            ] 
        
        let placeSecond f second first  =
            f(first,second)

        //used to map new controls for the FreeFlyController
        let MouseEventsMap (mess: FreeFlyController.Message) : MainState.Msg=
            let handlem but = 
                match but with
                |MouseButtons.Left -> None
                |MouseButtons.Right ->Some MouseButtons.Left
                |_-> None
           
            match mess with
            |FreeFlyController.Message.Down (but,pos)  -> 
                match but with
                |MouseButtons.Left -> FreeFlyController.Message.Nop
                |MouseButtons.Right -> (FreeFlyController.Message.Down (MouseButtons.Left,pos) )
                |_ -> mess
            |FreeFlyController.Message.Up but -> 
                match but with
                |MouseButtons.Left -> FreeFlyController.Message.Nop
                |MouseButtons.Right -> (FreeFlyController.Message.Up (MouseButtons.Left) )
                |_ -> mess
            |_ ->  mess
            |> MainState.CameraMessage
        let att =
            [
                style "position: fixed; left: 0; top: 0; width: 100%; height: 100%"
            ]
        require dependencies (
                body [] [
                    FreeFlyController.controlledControl m.cameraState (MouseEventsMap) frustum (AttributeMap.ofList att) sg

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
            //threads = MainState.Mutable.State.Lens.cameraState.Get >> FreeFlyController.threads >> ThreadPool.map MainState.CameraMessage
            threads = fun model -> FreeFlyController.threads model.cameraState |> ThreadPool.map MainState.CameraMessage
            unpersist = Unpersist.instance
        }