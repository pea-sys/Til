package main

func TakeUntil[K any](quit chan int, f func(K) bool, input <-chan K) <-chan K {
	output := make(chan K)
	go func() {
		defer close(output)
		moreData := true
		fValue := true
		var msg K
		for fValue && moreData {
			select {
			case msg, moreData = <-input:
				if moreData {
					fValue = f(msg)
					if fValue {
						output <- msg
					}
				}
			case <-quit:
				return
			}
		}
		if !fValue {
			close(quit)
		}
	}()
	return output
}
func main() {
	quit := make(chan int)
	input := make(chan int)
	output := TakeUntil(quit, func(x int) bool { return x < 5 }, input)
	input <- 1
	input <- 2
	input <- 3
	input <- 4
	input <- 5
	for x := range output {
		println(x)
	}
}
