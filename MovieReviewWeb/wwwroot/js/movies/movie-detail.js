document.addEventListener("DOMContentLoaded", async () => {

    const params = new URLSearchParams(window.location.search);
    const movieId = params.get("id");

    console.log("Movie Id : ", movieId);

    if (!movieId)
    {
        console.error("영화 ID가 없습니다.");

        return;
    }

    try
    {
        const response = await fetch(`https://localhost:7226/api/Movies/${movieId}`);

        if (!response.ok)
        {
            throw new Error(`영화 상세 정보 요청 실패 : ${response.status}`);
        }

        const movie = await response.json();
        console.log(movie);
        console.log("credits:", movie.credits);
        console.log("crew:", movie.credits?.crew);
        const cast  = movie.credits?.cast ?? [];
        const crew  = movie.credits?.crew ?? [];

        const director = crew.find(
            person => person.job === "Director"
        );

        console.log("감독 : ", director);

        const movieDetail = document.getElementById("movieDetail");
        const credits     = document.getElementById("cast-list");

        movieDetail.innerHTML = `
                                <div class="movie-detail">
                                    <img class="movie-detail-poster" src="https://image.tmdb.org/t/p/w500${movie.poster_path}" alt="${movie.title}">
                                    <div class="movie-detail-info">
                                        <h1 class="movie-detail-title">${movie.title}</h1>
                                        <div class="movie-detail-meta">
                                            <span class="movie-detail-rating">★ ${Number(movie.vote_average).toFixed(1)}</span>
                                            <span> ${movie.vote_count?.toLocaleString() || 0}명 평가 </span>
                                            <span>${movie.release_date || "개봉일 미정"}</span>
                                            <span>${movie.runtime || "-"}분</span>
                                        </div>
                                        <div class="movie-detail-genres">
                                            ${ movie.genres?.length ? movie.genres.map(genre => ` <span class="movie-genre"> ${genre.name} </span> `).join("") : `<span class="movie-genre">장르 정보 없음</span>` }
                                        </div>
                                        <div class="movie-detail-director">
                                            <span class="detail-label">감독: </span>
                                            <strong> ${director?.name || "감독 정보 없음"} </strong>
                                        </div>
                                        <p class="movie-detail-overview">${movie.overview || "등록된 영화 소개가 없습니다."}</p>
                                    </div>
                                </div>
                            `;
        // 출연진 출력
        renderCast(cast);
    }
    catch (error)
    {
        console.error("영화 상세 정보 요청 오류 : ", error);
    }
});

function renderCast(cast, showAll = false) {

    const credits    = document.getElementById("cast-list");
    const moreButton = document.getElementById("castMoreButton");

    const displayCast = showAll ? cast : cast.slice(0, 6);

    credits.innerHTML = displayCast.map(actor => {

        const profileUrl = actor.profile_path ? `https://image.tmdb.org/t/p/w500${actor.profile_path}` : "/images/no-profile.png";

        return `
                <div class="cast-item">

                    <img src="${profileUrl}" alt="${actor.name}">

                    <div class="cast-info">
                        <strong class="cast-name">${actor.name}</strong>
                        <span class="cast-character">${actor.character || ""}</span>
                    </div>
                </div>
            `;
    }).join("");

    if (cast.length <= 6) {
        moreButton.style.display = "none";

        return;
    }
    moreButton.style.display = "inline-flex";

    moreButton.textContent = showAll ? "접기" : "출연진 더보기";

    moreButton.onclick = () => {
        renderCast(cast, !showAll);
    };
}