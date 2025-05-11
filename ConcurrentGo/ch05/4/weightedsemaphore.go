package main

import (
	"sync"
)

type WeightedSemaphore struct {
	permits int
	cond    *sync.Cond
}

func (rw *WeightedSemaphore) Acquire(permits int) {
	rw.cond.L.Lock()
	for rw.permits-permits < 0 {
		rw.cond.Wait()
	}
	rw.permits -= permits
	rw.cond.L.Unlock()
}

func (rw *WeightedSemaphore) Release(permits int) {
	rw.cond.L.Lock()
	rw.permits += permits
	rw.cond.Signal()
	rw.cond.L.Unlock()
}
