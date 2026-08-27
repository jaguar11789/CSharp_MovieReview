// const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {

    // 검색
    document.getElementById("searchButton")
        ?.addEventListener("click", () => {
            loadUsers(1);
        });

    // 상태 필터
    document.getElementById("statusFilter")
        ?.addEventListener("change", () => {
            loadUsers(1);
        });

    // 가입일 필터
    document.getElementById("joinPeriodFilter")
        ?.addEventListener("change", () => {
            loadUsers(1);
        });

    // 페이지 크기
    document.getElementById("pageSizeSelect")
        ?.addEventListener("change", event => {

            pageSize = Number(event.target.value);

            loadUsers(1);
        });

    await loadUsers();

});

let currentPage = 1;
let pageSize = 10;

// =====================================================
// 회원 목록 조회
// =====================================================
async function loadUsers(page = 1)
{
    const userList = document.getElementById("userList");

    try
    {
        // 검색 조건
        const keyword    = document.getElementById("searchKeyword")?.value.trim() || "";
        const statusCode = document.getElementById("statusFilter")?.value || "";
        const joinDays   = document.getElementById("joinPeriodFilter")?.value || "";

        // Query String 생성
        const params = new URLSearchParams();

        params.set("page", page);
        params.set("pageSize", pageSize);

        if (keyword)
        {
            params.set("keyword", keyword);
        }

        if (statusCode)
        {
            params.set("statusCode", statusCode);
        }

        if (joinDays)
        {
            params.set("joinDays", joinDays);
        }

        const response = await fetch(`${API_BASE_URL}/api/AdminUser/users?${params.toString()}`,
            {
                credentials: "include"
            }
        );

        if (response.status === 401)
        {
            alert("로그인이 필요한 서비스입니다.");
            location.href = "/accounts/login";

            return;
        }
        if (response.status === 403)
        {
            alert("관리자 권한이 필요합니다.");
            location.href = "/main";

            return;
        }
        if (!response.ok)
        {
            throw new Error(`회원 목록 요청 실패 : ${response.status}`);
        }
        const users = await response.json();

        currentPage = users.page;

        renderUsers(users.items);
        renderPagination(users.page, users.totalPages);
    }
    catch (error)
    {
        console.error("회원 목록 조회 오류:", error);

        userList.innerHTML = `
                              <tr>
                                  <td colspan="6">
                                      회원 정보를 불러오지 못했습니다.
                                  </td>
                              </tr>
                             `;
    }
}

// =====================================================
// 회원 목록 출력
// =====================================================
function renderUsers(users)
{
    const userList = document.getElementById("userList");

    if (!users || users.length === 0)
    {
        userList.innerHTML = `
                              <tr>
                                 <td colspan="6">
                                    등록된 회원이 없습니다.
                                 </td>
                              </tr>
                             `;
        return;
    }
    userList.innerHTML = users.map(user => `
                                            <tr>
                                                <td class="user-id">${user.id}</td>
                                                <td class="user-account">
                                                    <a href="/admin/users/detail?id=${user.id}" class="user-detail-link user-account-link">${user.userId}</a>
                                                </td>
                                                <td class="user-email">
                                                    <a href="/admin/users/detail?id=${user.id}" class="user-detail-link">${user.email}</a>
                                                </td>
                                                <td>
                                                    <span class="user-role ${getRoleClass(user.role)}">
                                                        ${formatRole(user.role)}
                                                    </span>
                                                </td>
                                                <td class="user-date">${formatDate(user.createdAt)}</td>
                                                <td>
                                                    <span class="user-status ${getStatusClass(user.statusCode)}">
                                                        ${formatStatus(user.statusCode)}
                                                    </span>
                                                </td>
                                            </tr>
                                            `).join("");
}

// =====================================================
// 권한 CSS 클래스
// =====================================================
function getRoleClass(role) {
    switch (role) {
        case "Admin":
            return "admin";

        case "User":
            return "user";

        default:
            return "unknown";
    }
}

// =====================================================
// 상태 CSS 클래스
// =====================================================
function getStatusClass(statusCode) {
    switch (statusCode) {
        case 100:
            return "normal";

        case 200:
            return "dormant";

        case 300:
            return "suspended";

        case 900:
            return "withdrawn";

        default:
            return "unknown";
    }
}

// =====================================================
// 날짜
// =====================================================
function formatDate(date)
{
    if (!date)
    {
        return "";
    }
    return new Date(date).toLocaleDateString("ko-KR");
}

// =====================================================
// 권한
// =====================================================
function formatRole(role)
{
    switch (role)
    {
        case "Admin":
            return "관리자";
        case "User":
            return "사용자";
    }
}

// =====================================================
// 상태
// =====================================================
function formatStatus(statusCode)
{
    switch (statusCode)
    {
        case 100:
            return "정상";
        case 900:
            return "탈퇴";
        case 200:
            return "휴면";
        case 300:
            return "정지";
        default:
            return `상태 ${statusCode}`;
    }
}

// =====================================================
// 페이징
// =====================================================
function renderPagination(page, totalPages)
{
    const pagination = document.getElementById("userPagination");

    if (!pagination)
    {
        return;
    }

    if (totalPages <= 1)
    {
        pagination.innerHTML = "";
        return;
    }

    const pageBlockSize = 10;

    // 현재 페이지가 속한 블록
    const currentBlock = Math.floor((page - 1) / pageBlockSize);

    // 현재 블록의 시작 / 마지막 페이지
    const startPage = currentBlock * pageBlockSize + 1;
    const endPage   = Math.min(startPage + pageBlockSize - 1, totalPages);

    let html = "";

    // 첫 페이지
    html += `
            <button type="button" class="admin-page-button admin-page-arrow" onclick="loadUsers(1)" ${page === 1 ? "disabled" : ""} aria-label="첫 페이지">
                <i class="bi bi-chevron-double-left"></i>
            </button>
            `;

    // 이전 블록
    const previousBlockPage = startPage - 1;

    html += `
            <button type="button" class="admin-page-button admin-page-arrow" onclick="loadUsers(${previousBlockPage})" ${startPage === 1 ? "disabled" : ""} aria-label="이전 페이지">
                <i class="bi bi-chevron-left"></i>
            </button>
            `;

    // 페이지 번호
    for (let i = startPage; i <= endPage; i++) {
        html += `
                <button type="button" class="admin-page-button ${i === page ? "active" : ""}" onclick="loadUsers(${i})" aria-label="${i}페이지" ${i === page ? 'aria-current="page"' : ""}>${i}</button>
                `;
    }

    // 다음 블록
    const nextBlockPage = endPage + 1;

    html += `
            <button type="button" class="admin-page-button admin-page-arrow" onclick="loadUsers(${nextBlockPage})" ${endPage === totalPages ? "disabled" : ""} aria-label="다음 페이지">
                <i class="bi bi-chevron-right"></i>
            </button>
            `;

    // 마지막 페이지
    html += `
            <button type="button" class="admin-page-button admin-page-arrow" onclick="loadUsers(${totalPages})" ${page === totalPages ? "disabled" : ""} aria-label="마지막 페이지">
                <i class="bi bi-chevron-double-right"></i>
            </button>
            `;

    pagination.innerHTML = html;
}

/*function renderPagination(page, totalPages)
{
    let pagination = document.getElementById("userPagination");

    if (!pagination)
    {
        return;
    }

    if (totalPages <= 1)
    {
        pagination.innerHTML = "";
        return;
    }

    let html = "";

    for (let i = 1; i <= totalPages; i++)
    {
        html += `
                <button type="button" class="admin-page-button ${i === page ? "active" : ""}" onclick="loadUsers(${i})">${i}</button>
                `;
    }
    pagination.innerHTML = html;
}*/