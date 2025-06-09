package main

import (
	"sort"
	"sync"
)

type Player struct {
	name  string
	score int
	mutex sync.Mutex
}

func incrementScores(players []*Player, increment int) {
	sort.Slice(players, func(a, b int) bool {
		return players[a].name < players[b].name
	})
	for _, player := range players {
		player.mutex.Lock()
	}
	for _, player := range players {
		player.score += increment
	}
	for _, player := range players {
		player.mutex.Unlock()
	}
}
