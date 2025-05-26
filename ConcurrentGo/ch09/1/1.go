package main

func GenerateSquares(quit <-chan int) <-chan int {
	squares := make(chan int)
	go func() {
		i := 0
		defer close(squares)
		for {
			i += 1
			select {
			case squares <- i * i:
			case <-quit:
				return
			}
		}
	}()
	return squares
}

func main() {
	quit := make(chan int)
	squares := GenerateSquares(quit)
	for i := 0; i < 10; i++ {
		println(<-squares)
	}
	close(quit)
}
