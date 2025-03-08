build:
	docker-buildx build --plataform linux/amd64,linux/arm64 -t chatapp .