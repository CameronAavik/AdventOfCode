namespace AdventOfCode.FSharp

open System.Collections.Generic

// GraphFS library imported from https://github.com/CameronAavik/GraphFS
module GraphFS =
    module MapHelpers =
        let inline private _GetOrDefault f key ``default`` (d : Map<'a, 'b>) =
            match d.TryGetValue key with
            | true, value -> value |> f
            | false, _ -> ``default``

        let inline GetOrDefault k1 ``default`` d = _GetOrDefault id k1 ``default`` d

        let inline GetOrThrow2 (k1, k2) (d : Map<'a, Map<'b, 'c>>) = d.[k1].[k2]

        let inline private _ContainsKey f key (d : Map<'a, 'b>) =
            match d.TryGetValue key with
            | true, value -> value |> f
            | false, _ -> false

        let inline ContainsKey k1 (d : Map<'a, 'b>) = d.ContainsKey k1
        let inline ContainsKey2 (k1, k2) d = _ContainsKey (ContainsKey k2) k1 d

        let inline private _Add f k v d =
            let d2 = GetOrDefault k Map.empty d
            Map.add k (f v d2) d
    
        let inline Add k v d = Map.add k v d
        let inline Add2 (k1, k2) v d = _Add (Add k2) k1 v d 

        let inline private _ApplyOrEmptySeq f key (d : Map<'a, 'b>) =
            match d.TryGetValue key with
            | true, value -> value |> f
            | false, _ -> Seq.empty

        let inline Keys (d : Map<'a, 'b>) : 'a seq = Map.toSeq d |> Seq.map fst
        let inline Keys2 k d = _ApplyOrEmptySeq Keys k d

        let inline AsTupleSeq (d : Map<'a, 'b>) = d |> Seq.map (fun kvp -> (kvp.Key, kvp.Value))
        let inline AsTupleSeq2 k d = _ApplyOrEmptySeq AsTupleSeq k d


    module Edge =
        let inline edgeToVerts (u, v) = [|u; v|]
        let inline edgeWithDataToVerts ((u, v), _) = [|u; v|]

        let inline vertsFromEdgeSeq (toVerts : 'a -> 'b array) edges =
            let vertSet = new HashSet<'b>()
            for edge in edges do
                let vertArr = toVerts edge
                vertSet.Add vertArr.[0] |> ignore
                vertSet.Add vertArr.[1] |> ignore
            vertSet |> Seq.toArray

        let inline edgesToVerts edges =  vertsFromEdgeSeq edgeToVerts edges
        let inline edgesWithDataToVerts edges = vertsFromEdgeSeq edgeWithDataToVerts edges

    module VertexSet =
        type IVertexSet<'V> =
            abstract member HasVert : 'V -> bool

        type IFiniteVertexSet<'V> =
            inherit IVertexSet<'V>
            abstract member Verts : 'V seq

        type VertexSet<'V when 'V : comparison>(verts : Set<'V>) =
            member __.AddVert vert = new VertexSet<'V>(verts.Add vert)
            member __.AddVerts toAdd = new VertexSet<'V>(verts + (set toAdd))
            interface IFiniteVertexSet<'V> with
                member __.HasVert v = verts.Contains v
                member __.Verts = upcast verts

        module VertexSet =
            let empty<'V when 'V : comparison> =  new VertexSet<'V>(Set.empty)

            let inline hasVert vertex (vertexSet : IVertexSet<'V>) = vertexSet.HasVert vertex
            let inline verts (vertexSet : IFiniteVertexSet<'V>) = vertexSet.Verts
            let inline addVert vert vertexSet = (^VS : (member AddVert : 'V -> ^VS) (vertexSet, vert))
            let inline addVerts verts vertexSet = (^VS : (member AddVerts : 'V seq -> ^VS) (vertexSet, verts))

    module EdgeSet =
        type IEdgeSet<'V> =
            abstract member Neighbours : 'V -> 'V seq
            abstract member HasEdge : ('V * 'V) -> bool

        type IEdgeWithDataSet<'V, 'E> =
            inherit IEdgeSet<'V>
            abstract member NeighboursWithData : 'V -> ('V * 'E) seq
            abstract member GetEdgeData : ('V * 'V) -> 'E

        type EdgeWithDataSet<'V, 'E when 'V : comparison and 'V : not null>(edges : Map<'V, Map<'V, 'E>>) =
            member __.AddEdgeWithData edgeWithData =
                let edge, data = edgeWithData
                MapHelpers.Add2 edge data edges |> EdgeWithDataSet<'V, 'E>
            member es.AddEdgesWithData edges = (Seq.fold (fun (es : EdgeWithDataSet<'V, 'E>) (edge, data) -> es.AddEdgeWithData (edge, data)) es edges)
            interface IEdgeWithDataSet<'V, 'E> with
                member __.Neighbours n = MapHelpers.Keys2 n edges
                member __.HasEdge neighbour = MapHelpers.ContainsKey2 neighbour edges
                member __.NeighboursWithData n = MapHelpers.AsTupleSeq2 n edges
                member __.GetEdgeData neighbour = MapHelpers.GetOrThrow2 neighbour edges

        module EdgeWithDataSet =
            let empty<'V, 'E when 'V : comparison and 'V : not null> = new EdgeWithDataSet<'V, 'E>(Map.empty)

        type EdgeSet<'V when 'V : comparison and 'V : not null>(edges : Map<'V, Map<'V, unit>>) =
            member __.AddEdge edge = MapHelpers.Add2 edge () edges |> EdgeSet<'V>
            member es.AddEdges edge = Seq.fold (fun (es : EdgeSet<'V>) n -> es.AddEdge n) es edge
            interface IEdgeSet<'V> with
                member __.Neighbours n = MapHelpers.Keys2 n edges
                member __.HasEdge neighbour = MapHelpers.ContainsKey2 neighbour edges

        module EdgeSet =
            let empty<'V when 'V : comparison and 'V : not null> = new EdgeSet<'V>(Map.empty)

            let inline neighbours vertex (edgeSet : IEdgeSet<'V>) = edgeSet.Neighbours vertex
            let inline hasEdge edge (edgeSet : IEdgeSet<'V>) = edgeSet.HasEdge edge
            let inline neighboursWithData vertex (edgeSet : IEdgeWithDataSet<'V, 'E>) = edgeSet.NeighboursWithData vertex
            let inline getEdgeData edge (edgeSet : IEdgeWithDataSet<'V, 'E>) = edgeSet.GetEdgeData edge
            let inline addEdge edge edgeSet = (^ES : (member AddEdge : ('V * 'V) -> ^ES) (edgeSet, edge))
            let inline addEdges edges edgeSet = (^ES : (member AddEdges : ('V * 'V) seq -> ^ES) (edgeSet, edges))
            let inline addEdgeWithData edge data edgeSet = (^ES : (member AddEdgeWithData : (('V * 'V) * 'E) -> ^ES) (edgeSet, (edge, data)))
            let inline addEdgesWithData edges edgeSet = (^ES : (member AddEdgesWithData : (('V * 'V) * 'E) seq -> ^ES) (edgeSet, edges))

    module Graph =
        open VertexSet
        open EdgeSet

        type Graph<'V, 'VS, 'ES when 'VS :> IVertexSet<'V> and 'ES :> IEdgeSet<'V>>(V: 'VS, E: 'ES) =
            member __.V = V
            member __.E = E
            static member applyV f (g : Graph<'V, 'VS, 'ES>) = f g.V
            static member applyE f (g : Graph<'V, 'VS, 'ES>) = f g.E
            static member mapV f (g : Graph<'V, 'VS, 'ES>) = Graph(f g.V, g.E)
            static member mapE f (g : Graph<'V, 'VS, 'ES>) = Graph(g.V, f g.E)
            static member mapVE f1 f2 (g : Graph<'V, 'VS, 'ES>) = Graph(f1 g.V, f2 g.E)

        module Graph =
            let inline hasVert vert G = Graph.applyV (VertexSet.hasVert vert) G
            let inline verts G = Graph.applyV VertexSet.verts G
            let inline neighbours vert G  = Graph.applyE (EdgeSet.neighbours vert) G
            let inline hasEdge edge G = Graph.applyE (EdgeSet.hasEdge edge) G
            let inline neighboursWithData vert G = Graph.applyE (EdgeSet.neighboursWithData vert) G
            let inline getEdgeData edge G = Graph.applyE (EdgeSet.getEdgeData edge) G
            let inline addVert vert G = Graph.mapV (VertexSet.addVert vert) G
            let inline addVerts verts G = Graph.mapV (VertexSet.addVerts verts) G
            let inline addEdge edge G = Graph.mapVE (VertexSet.addVerts (Edge.edgeToVerts edge)) (EdgeSet.addEdge edge) G
            let inline addEdges edges G = Graph.mapVE (VertexSet.addVerts (Edge.edgesToVerts edges)) (EdgeSet.addEdges edges) G
            let inline addEdgeWithData edge data G = Graph.mapVE (VertexSet.addVerts (Edge.edgeToVerts edge)) (EdgeSet.addEdgeWithData edge data) G
            let inline addEdgesWithData edges G = Graph.mapVE (VertexSet.addVerts (Edge.edgesWithDataToVerts edges)) (EdgeSet.addEdgesWithData edges) G
        
            let fromSets V E = Graph (V, E)
            let empty<'V when 'V : comparison and 'V : not null> = Graph (VertexSet.empty<'V>, EdgeSet.empty<'V>)
            let emptyWithEdgeData<'V, 'E when 'V : comparison and 'V : not null> = Graph (VertexSet.empty<'V>, EdgeWithDataSet.empty<'V, 'E>)
            let fromEdges<'V when 'V : comparison and 'V : not null> edges = empty<'V> |> addEdges edges
            let fromEdgesWithData<'V, 'E when 'V : comparison and 'V : not null> edges = emptyWithEdgeData<'V, 'E> |> addEdgesWithData edges

    module ShortestPath =
        open Graph

        let inline private _dijkstra source target weightFunc graph =
            let seen = new HashSet<_>()

            let rec dijkstraInner fringe =
                if Set.isEmpty fringe then None
                else
                    let dist, vertex = Set.minElement fringe
                    let fringe' = Set.remove (dist, vertex) fringe
                    if seen.Contains(vertex) then dijkstraInner fringe'
                    elif vertex = target then Some dist
                    else
                        seen.Add vertex |> ignore
                        Graph.neighbours vertex graph
                        |> Seq.map (fun v -> (dist + weightFunc (vertex, v), v))
                        |> Set.ofSeq
                        |> Set.union fringe'
                        |> dijkstraInner

            set [(0, source)] |> dijkstraInner

        let inline dijkstra source target graph = _dijkstra source target (fun _ -> 1) graph
        let inline weightedDijkstra source target graph =
            let inline getWeight edge =
                let edgeData = Graph.getEdgeData edge graph
                (^E : (member Weight : int) edgeData)
            _dijkstra source target getWeight graph

        let private _astar source target heuristic weightFunc graph =
            let seen = new HashSet<_>()
            let h v = heuristic v target

            let rec astarInner fringe =
                if Set.isEmpty fringe then None
                else
                    let (_, negDist, vertex) as minElem = Set.minElement fringe
                    let fringe' = Set.remove minElem fringe
                    let dist = -negDist
                    if seen.Contains(vertex) then astarInner fringe'
                    elif vertex = target then Some dist
                    else
                        seen.Add vertex |> ignore
                        Graph.neighbours vertex graph
                        |> Seq.map (fun v -> (dist + weightFunc (vertex, v), v))
                        |> Seq.map (fun (dist, v) -> ((h v) + dist, -dist, v))
                        |> Set.ofSeq
                        |> Set.union fringe'
                        |> astarInner
                    
            set [(h source, 0, source)] |> astarInner

        let astar source target heuristic graph = _astar source target heuristic (fun _ -> 1) graph
