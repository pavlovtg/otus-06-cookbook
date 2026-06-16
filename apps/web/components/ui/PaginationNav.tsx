"use client";

import { useRouter } from "next/navigation";
import { Pagination } from "@/components/ui/Pagination";

export function PaginationNav({
  page,
  totalPages,
}: {
  page: number;
  totalPages: number;
}) {
  const router = useRouter();
  return (
    <Pagination
      page={page}
      total={totalPages}
      onChange={(p) => router.push(`?page=${p}`)}
    />
  );
}
