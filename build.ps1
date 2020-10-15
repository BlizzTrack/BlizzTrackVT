docker build --rm --file '.\BlizzTrackVT\Dockerfile' . --tag registry.gitlab.com/toyz/vt:latest
docker build --rm --file '.\BTVT_Worker\Dockerfile' . --tag registry.gitlab.com/toyz/vt/worker:latest

docker push registry.gitlab.com/toyz/vt:latest
docker push registry.gitlab.com/toyz/vt/worker:latest

docker image prune -f
