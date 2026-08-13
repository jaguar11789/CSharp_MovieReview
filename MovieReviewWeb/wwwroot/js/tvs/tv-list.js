const API_BASE_URL = "https://localhost:7226";

const tvGenres = [
    { id: 0,     name: "전체" },
    { id: 10759, name: "액션 & 모험" },
    { id: 16,    name: "애니메이션" },
    { id: 35,    name: "코미디" },
    { id: 80,    name: "범죄" },

    { id: 99,    name: "다큐멘터리" },
    { id: 18,    name: "드라마" },
    { id: 10751, name: "가족" },
    { id: 10762, name: "키즈" },
    { id: 9648,  name: "미스터리" },

    { id: 10763, name: "뉴스" },
    { id: 10764, name: "리얼리티" },
    { id: 10765, name: "SF & 판타지" },
    { id: 10766, name: "Soap" },
    { id: 10767, name: "토크" },

    { id: 10768, name: "전쟁 & 정치" },
    { id: 37,    name: "서부" }
];

let currentMode        = "all";
let currentGenreId     = 0;
let currentSearchQuery = "";

document.addEventListener("DOMContentLoaded", async () => {

    renderGenres();

    const searchInput  = document.getElementById("tvSearchInput");
    const searchButton = document.getElementById("tvSearchButton");

    searchButton.addEventListener("click", async () => {

        const query = searchInput.value.trim();

        if (!query)
        {
            alert("검색어를 입력해주세요.");

            return;
        }
        currentMode        = "search";
        currentSearchQuery = query;

        searchTvs(query, 1);
    });

    searchInput.addEventListener("keydown", event => {

        if (event.key === "Enter")
        {
            searchButton.click();
        }
    });

    await loadTvs();
});

// ==================================================
// TV 프로그램 출력
// ==================================================

function renderTvs(tvs)
{
    const tvList = document.getElementById("tvList");

    if (!tvs?.length)
    {
        tvList.innerHTML = `<div class="movie-error"> TV 프로그램 정보가 없습니다. </div>`;

        return;
    }

    tvList.innerHTML = tvs.map(tv => {
        const posterUrl = tv.poster_path ? `https://image.tmdb.org/t/p/w500${tv.poster_path}` : "image/no-poster.png";

        return `
                <div class="movie-card" onclick="goToTv(${tv.id})">
                    <img src="${posterUrl}" alt="${tv.name}">
                    <div class="movie-info">
                        <h3>${tv.name}</h3>
                        <span class="movie-rating">★ ${Number(tv.vote_average).toFixed(1)}</span>
                        <span class="movie-date">${tv.release_date || ""}</span>
                    </div>
                </div>
               `;
    }).join("");
}

async function loadTvs(page = 1)
{

    try {
        const response = await fetch(`${API_BASE_URL}/api/Tvs?page=${page}`);

        if (!response.ok) {
            throw new Error(`TV 프로그램 요청 실패: ${response.status}`);
        }

        const tvData = await response.json();

        console.log("zz");
        console.log(tvData);

        renderTvs(tvData.results);
        renderPagination(tvData, page => loadTvs(page));
    }
    catch (error) {
        console.error("전체 TV 프로그램 요청 오류 :", error);

        const tvList = document.getElementById("tvList");

        tvList.innerHTML = `
                            <div class="tv-error">
                                영화 정보를 불러오지 못했습니다.
                            </div>
                            `;
    }
}

// ==================================================
// 장르별 TV 프로그램
// ==================================================
async function loadTvsByGenre(genreId, page = 1)
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Tvs/genre/${genreId}?page=${page}`);

        if (!response.ok)
        {
            throw new Error(`장르별 TV 프로그램 요청 실패 : ${response.status}`);
        }

        const tvData = await response.json();

        renderTvs(tvData.results);
        renderPagination(tvData, page => loadTvsByGenre(genreId, page));
    }
    catch (error)
    {
        console.error("장르별 영화 요청 오류 :", error);
    }    
}

// ==================================================
// 장르 버튼
// ==================================================

function renderGenres()
{
    const genreContainer = document.getElementById("tvGenres");

    genreContainer.innerHTML = tvGenres.map((genre, index) => {

        return `<button type="button" class="genre-button ${index === 0 ? "active" : ""}" data-genre-id="${genre.id}">${genre.name}</button>`;
    }).join("");

    genreContainer.querySelectorAll(".genre-button").forEach(button => {

        button.addEventListener("click", async () => {

            genreContainer.querySelectorAll(".genre-button").forEach(btn => {
                btn.classList.remove("active");
            });
            button.classList.add("active");

            const genreId = Number(button.dataset.genreId);

            if (genreId === 0)
            {
                currentMode        = "all";
                currentGenreId     = 0;
                currentSearchQuery = "";

                await loadTvs(1);

                return;
            }
            currentMode        = "genre";
            currentGenreId     = genreId;
            currentSearchQuery = "";

            await loadTvsByGenre(genreId, 1);
        });
    });
}

// ==================================================
// 검색
// ==================================================
async function searchTvs(query, page = 1)
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Tvs/search?query=${encodeURIComponent(query)}&page=${page}`);

        if (!response.ok)
        {
            throw new Error(`TV 프로그램 검색 실패 : ${response.status}`);
        }

        const tvData = await response.json();

        renderTvs(tvData.results);
        renderPagination(tvData, page => searchTvs(query, page));
    }
    catch (error)
    {
        console.error("영화 검색 오류 :", error);
    }
}

// ==================================================
// 페이징
// ==================================================
function renderPagination(pageData, loadPage) {
    const pagination = document.getElementById("tvPagination");

    pagination.innerHTML = "";

    const { currentPage, totalPages, startPage, endPage } = pageData;

    if (totalPages <= 1) {
        return;
    }

    // 이전 버튼
    if (startPage > 1)
    {

        const prevButton = document.createElement("button");

        prevButton.type        = "button";
        prevButton.className   = "page-button";
        prevButton.textContent = "‹";

        prevButton.addEventListener("click", () => {
            loadPage(startPage - 1);
        });

        pagination.appendChild(prevButton);
    }

    // 페이지 번호
    for (let page = startPage; page <= endPage; page++)
    {
        const pageButton = document.createElement("button");

        pageButton.type      = "button";
        pageButton.className = `page-button ${page === currentPage ? "active" : ""}`;

        pageButton.textContent = page;

        pageButton.addEventListener("click", () => {
            loadPage(page);
        });

        pagination.appendChild(pageButton);
    }

    // 다음 버튼
    if (endPage < totalPages)
    {
        const nextButton = document.createElement("button");

        nextButton.type        = "button";
        nextButton.className   = "page-button";
        nextButton.textContent = "›";

        nextButton.addEventListener("click", () => {
            loadPage(endPage + 1);
        });

        pagination.appendChild(nextButton);
    }
}

function goToTv(tvId) {
    window.location.href = `/tvs/detail?id=${tvId}`;
}