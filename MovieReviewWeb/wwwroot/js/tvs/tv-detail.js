const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {

    const params = new URLSearchParams(window.location.search);
    const tvId = params.get("id");

    console.log("TV Id : ", tvId);

    if (!tvId) {
        console.log("TV 프로그램 ID가 없습니다.");

        return;
    }
    try {
        const response = await fetch(`${API_BASE_URL}/api/Tvs/${tvId}`);

        if (!response.ok) {
            throw new Error(`TV 프로그램 상세 정보 요청 실패 : ${response.status}`);
        }

        const tv = await response.json();
        console.log("TV 프로그램 상세 정보 : ", tv);

        const tvDetail       = document.getElementById("tvDetail");
        const credits        = document.getElementById("cast-list");
        const genres         = tv.genres?.length ? tv.genres.map(genre => `<span class="movie-genre">${genre.name}</span>`).join("") : `<span class="movie-genre">장르 정보 없음</span>`;
        const creators       = tv.created_by?.length ? tv.created_by.map(creator => creator.name).join(", ") : "제작자 정보 없음";
        const episodeRuntime = tv.episode_run_time?.length ? `${tv.episode_run_time[0]}분` : "-";
        console.log("출연진 정보 : ", tv.credits?.cast);
        console.log("출연진 정보 : ", tv.credits);

        tvDetail.innerHTML = `
                             <div class="movie-detail">

                                <img class="movie-detail-poster" src="https://image.tmdb.org/t/p/w500${tv.poster_path}" alt="${tv.name}">
                                <div class="movie-detail-info">
                                    <h1 class="movie-detail-title">${tv.name}</h1>

                                    <div class="movie-detail-meta">
                                        <span class="movie-detail-rating">★ ${Number(tv.vote_average).toFixed(1)}</span>
                                        <span>${tv.vote_count?.toLocaleString() || 0}명 평가</span>
                                        <span>${tv.first_air_date || "방영일 미정"}</span>
                                        <span>${episodeRuntime}</span>
                                    </div>

                                    <div class="movie-detail-genres">
                                        ${genres}
                                    </div>

                                    <div class="movie-detail-director">
                                        <span class="detail-label">제작:</span>
                                        <strong>${creators}</strong>
                                    </div>
                                    <p class="movie-detail-overview">${tv.overview || "등록된 TV 프로그램 소개가 없습니다."}</p>
                                    <div class="movie-detail-actions">
                                        <button type="button" class="movie-review-button" onclick="goToTvReview(${tv.id})">★ 리뷰 보기</button>
                                    </div>
                                </div>
                            </div>
                             `;
        renderTvCast(tv.credits?.cast ?? []);
    }
    catch (error) {
        console.error("TV 상세 정보 요청 오류 :", error);
    }
});

function renderTvCast(cast, showAll = false) {

    const castList   = document.getElementById("tvCastList");
    const moreButton = document.getElementById("tvCastMoreButton");

    const displayCast = showAll ? cast : cast.slice(0, 6);

    castList.innerHTML = displayCast.map(actor => {

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
        renderTvCast(cast, !showAll);
    };
}