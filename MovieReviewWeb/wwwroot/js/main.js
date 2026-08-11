const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {
    await loadPopularMoviesAndTvs();
});


async function loadPopularMoviesAndTvs()
{

    const featuredMovieContainer = document.getElementById("featuredMovie");
    const movieContainer         = document.getElementById("popularMovies");
    const tvContainer            = document.getElementById("popularTv");

    try {
        const movieResponse = await fetch(`${API_BASE_URL}/api/Movies/popular`);
        const tvResponse    = await fetch(`${API_BASE_URL}/api/Tvs/popular`);

        if (!movieResponse.ok)
        {
            throw new Error(`인기 영화 요청 실패: ${movieResponse.status} `);
        }

        if (!tvResponse.ok) {
            throw new Error(`인기 TV 프로그램 요청 실패: ${tvResponse.status} `);
        }

        const movieData = await movieResponse.json();
        const tvData    = await tvResponse.json();

        if (!movieData.results?.length)
        {
            throw new Error("영화 데이터가 없습니다.");
        }
        if (!tvData.results?.length)
        {
            throw new Error("TV 프로그램 데이터가 없습니다.");
        }
        // 첫 번째 영화 → 대표 영화
        const featuredMovie = movieData.results[0];

        renderFeaturedMovie(featuredMovieContainer, featuredMovie);
        // 나머지 영화 → 인기 영화 목록
        renderPopularMovies(movieContainer, movieData.results.slice(1));
        renderPopularTvs(tvContainer, tvData.results);

        console.log(tvData)
    }
    catch (error)
    {
        console.error(error);

        featuredMovieContainer.innerHTML = `
                                            <div class="movie-error">
                                                대표 영화 정보를 불러오지 못했습니다.
                                            </div >
                                           `;

        movieContainer.innerHTML = `
                                    <div class="movie-error">
                                        인기 영화 정보를 불러오지 못했습니다.
                                    </div >
                                   `;

        tvContainer.innerHTML = `
                                <div class="movie-error">
                                    인기 TV 프로그램 정보를 불러오지 못했습니다.
                                </div>
                                `;
    }
}


function renderFeaturedMovie(container, movie)
{

const backdropUrl = movie.backdrop_path
        ? `https://image.tmdb.org/t/p/original${movie.backdrop_path}`
        : `https://image.tmdb.org/t/p/w1280${movie.poster_path}`;


const posterUrl = movie.poster_path ? `https://image.tmdb.org/t/p/w500${movie.poster_path}` : "";


container.innerHTML = `
        <div class="featured-movie" style="background-image: linear-gradient(90deg, rgba(0,0,0,0.95) 0%, rgba(0,0,0,0.75) 35%, rgba(0,0,0,0.25) 75%, rgba(8,8,8,0.9) 100%), url('${backdropUrl}');">
            <div class="featured-content">
                ${posterUrl? `<img class="featured-poster" src="${posterUrl}" alt="${movie.title}">`: ""}
                <div class="featured-info">
                    <h1>${movie.title}</h1>
                    <div class="featured-meta">
                        <span>★ ${Number(movie.vote_average).toFixed(1)}</span>
                        <span>${movie.release_date || "개봉일 미정"}</span>
                    </div>
                    <!-- 영화 줄거리 -->
                    <p class="featured-overview">${movie.overview || "등록된 영화 소개가 없습니다."}</p>
                    <div class="featured-actions">
                        <button type="button" class="featured-button" onclick="goToMovie(${movie.id})">▶ 상세보기</button>
                        <button type="button" class="review-button" onclick="goToMovieReview(${movie.id})" > ★ 리뷰 보기 </button>
                    </div>
                </div>
            </div>
        </div>`;
}

function goToMovieReview(movieId)
{
    window.location.href = `/movies/reviews?movieId=${movieId}`;
}

function renderPopularMovies(container, movies)
{

    container.innerHTML = "";

    movies.forEach(movie => {

        const movieCard = document.createElement("div");

        movieCard.className = "movie-card";

        const posterUrl = movie.poster_path ? `https://image.tmdb.org/t/p/w500${movie.poster_path}` : "/image/no-poster.png";

        movieCard.innerHTML = `
            <img src="${posterUrl}" alt="${movie.title}">

            <div class="movie-info">
                <h3>${movie.title}</h3>
                <span class="movie-rating">★ ${Number(movie.vote_average).toFixed(1)}</span>
                <span class="movie-date">${movie.release_date || ""}</span>
            </div>`;

        movieCard.addEventListener("click", () => goToMovie(movie.id));
        container.appendChild(movieCard);

    });
    initializePopularSlider();
}

function renderPopularTvs(container, tvs)
{
    container.innerHTML = "";

    tvs.forEach(tv => {
        const tvCard = document.createElement("div");

        tvCard.className = "movie-card";

        const posterUrl = tv.poster_path ? `https://image.tmdb.org/t/p/w500${tv.poster_path}` : "/image/no-poster.png";

        tvCard.innerHTML = `
            <img src="${posterUrl}" alt="${tv.name}">

            <div class="movie-info">
                <h3>${tv.name}</h3>
                <span class="movie-rating">★ ${Number(tv.vote_average).toFixed(1)}</span>
                <span class="movie-date">${tv.release_date || ""}</span>
            </div>`;

        tvCard.addEventListener("click", () => goToTv(tv.id));
        container.appendChild(tvCard);
    });
    initializePopularTvSlider();
}

function goToMovie(movieId)
{
    window.location.href = `/movies/detail?id=${movieId}`;
}
function goToTv(tvId) {

    window.location.href = `/tvs/detail?id=${tvId}`;
}
let popularCurrentPage = 0;

// 영화 슬라이더
function initializePopularSlider()
{

    const slider     = document.getElementById("popularMovies");
    const prevButton = document.getElementById("popularPrev");
    const nextButton = document.getElementById("popularNext");


    if (!slider || !prevButton || !nextButton)
    {
        return;
    }


    function getPageSize()
    {

        if (window.innerWidth <= 650)
        {
            return 2;
        }

        if (window.innerWidth <= 1000)
        {
            return 4;
        }

        return 5;
    }


    function updateButtons()
    {

        const maxScroll = slider.scrollWidth - slider.clientWidth;

        prevButton.disabled = slider.scrollLeft <= 0;
        nextButton.disabled = slider.scrollLeft >= maxScroll - 2;
    }


    function moveNext()
    {

        const pageSize = getPageSize();
        const cards    = slider.querySelectorAll(".movie-card");

        if (cards.length === 0)
        {

            return;
        }
        /*
         * 현재 화면에 표시되는 카드 개수만큼 이동
         */
        const firstCard    = cards[0];
        const cardWidth    = firstCard.offsetWidth;
        const style        = window.getComputedStyle(slider);
        const gap          = parseFloat(style.gap) || 0;
        const moveDistance = (cardWidth + gap) * pageSize;


        /*
         * 마지막 부분까지 남은 거리가
         * pageSize보다 적으면
         * scrollWidth 끝까지 이동
         */
        const maxScroll    = slider.scrollWidth - slider.clientWidth;
        const nextPosition = Math.min(slider.scrollLeft + moveDistance, maxScroll);

        slider.scrollTo({
            left: nextPosition,
            behavior: "smooth"
        });
    }

    function movePrevious()
    {

        const pageSize = getPageSize();
        const cards    = slider.querySelectorAll(".movie-card");

        if (cards.length === 0)
        {
            return;
        }

        const cardWidth        = cards[0].offsetWidth;
        const style            = window.getComputedStyle(slider);
        const gap              = parseFloat(style.gap) || 0;
        const moveDistance     = (cardWidth + gap) * pageSize;
        const previousPosition = Math.max(slider.scrollLeft - moveDistance, 0);


        slider.scrollTo({
            left: previousPosition,
            behavior: "smooth"
        });
    }

    prevButton.onclick = movePrevious;
    nextButton.onclick = moveNext;

    slider.addEventListener("scroll", updateButtons);
    window.addEventListener("resize", updateButtons);

    updateButtons();
}

// TV 슬라이더
function initializePopularTvSlider() {
    const slider =
        document.getElementById("popularTv");

    const prevButton =
        document.getElementById("popularTvPrev");

    const nextButton =
        document.getElementById("popularTvNext");

    if (!slider || !prevButton || !nextButton) {
        return;
    }

    function getPageSize() {
        if (window.innerWidth <= 650) {
            return 2;
        }

        if (window.innerWidth <= 1000) {
            return 4;
        }

        return 5;
    }

    function updateButtons() {
        const maxScroll =
            slider.scrollWidth - slider.clientWidth;

        prevButton.disabled =
            slider.scrollLeft <= 0;

        nextButton.disabled =
            slider.scrollLeft >= maxScroll - 2;
    }

    function moveNext() {
        const pageSize = getPageSize();

        const cards =
            slider.querySelectorAll(".movie-card");

        if (cards.length === 0) {
            return;
        }

        const cardWidth =
            cards[0].offsetWidth;

        const style =
            window.getComputedStyle(slider);

        const gap =
            parseFloat(style.gap) || 0;

        const moveDistance =
            (cardWidth + gap) * pageSize;

        const maxScroll =
            slider.scrollWidth - slider.clientWidth;

        const nextPosition =
            Math.min(
                slider.scrollLeft + moveDistance,
                maxScroll
            );

        slider.scrollTo({
            left: nextPosition,
            behavior: "smooth"
        });
    }

    function movePrevious() {
        const pageSize = getPageSize();

        const cards =
            slider.querySelectorAll(".movie-card");

        if (cards.length === 0) {
            return;
        }

        const cardWidth =
            cards[0].offsetWidth;

        const style =
            window.getComputedStyle(slider);

        const gap =
            parseFloat(style.gap) || 0;

        const moveDistance =
            (cardWidth + gap) * pageSize;

        const previousPosition =
            Math.max(
                slider.scrollLeft - moveDistance,
                0
            );

        slider.scrollTo({
            left: previousPosition,
            behavior: "smooth"
        });
    }

    prevButton.onclick = movePrevious;
    nextButton.onclick = moveNext;

    slider.addEventListener(
        "scroll",
        updateButtons
    );

    window.addEventListener(
        "resize",
        updateButtons
    );

    updateButtons();
}